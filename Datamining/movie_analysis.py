# Подключение всех необходимых библиотек для анализа, машинного обучения и визуализации
# pandas и numpy — для работы с данными
# sklearn — для подготовки данных, построения моделей и оценки качества
# matplotlib и seaborn — для построения графиков
# mlxtend — для поиска ассоциативных правил
import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import MultiLabelBinarizer, StandardScaler
from sklearn.ensemble import RandomForestRegressor
from sklearn.linear_model import LinearRegression
from sklearn.metrics import mean_absolute_error, mean_squared_error, r2_score
from sklearn.decomposition import PCA
from sklearn.cluster import KMeans
import matplotlib.pyplot as plt
import seaborn as sns
from mlxtend.frequent_patterns import apriori, association_rules

# Функция для очистки и базовой предобработки датафрейма с фильмами
# - Удаляет строки с пропущенными ключевыми значениями
# - Преобразует жанры в списки
# - Извлекает длительность фильма в минутах
# - Преобразует числовые признаки к числовому типу
# - Оставляет только топ-10 режиссеров, остальные помечает как 'Other'
def preprocess_dataframe(df):
    df = df.dropna(subset=['Genre', 'IMDb Rating', 'Duration']).copy()
    df['Genre'] = df['Genre'].apply(lambda x: [g.strip() for g in str(x).split(',')])
    df['Duration_min'] = df['Duration'].str.extract(r'(\d+)').astype(int)
    for col in ['Metascore', 'Votes', 'Grossed in $']:
        df[col] = pd.to_numeric(df[col], errors='coerce')
    top_directors = df['Director'].value_counts().nlargest(10).index
    for col in ['Director', 'Certificate']:
        df[col] = df[col].fillna('Unknown')
    df['Director'] = df['Director'].apply(lambda x: x if x in top_directors else 'Other')
    return df, top_directors

# Функция для кодирования признаков:
# - Жанры кодируются мульти-бинаризатором (каждый жанр — отдельный столбец)
# - Категориальные признаки (сертификат, режиссер) кодируются one-hot методом
# - Возвращает итоговую матрицу признаков и объекты кодировщиков
# mlb — MultiLabelBinarizer для жанров, df_cert и df_dir — one-hot датафреймы
# top_directors не используется, но может быть полезен для расширения
def encode_features(df, mlb=None, top_directors=None):
    if mlb is None:
        mlb = MultiLabelBinarizer()
        X_genre = mlb.fit_transform(df['Genre'])
    else:
        X_genre = mlb.transform(df['Genre'])
    df_cert = pd.get_dummies(df['Certificate'], prefix='Cert')
    df_dir = pd.get_dummies(df['Director'], prefix='Dir')
    X = pd.concat([
        pd.DataFrame(X_genre, columns=mlb.classes_, index=df.index),
        df[['Duration_min', 'Grossed in $']].fillna(0),
        df_cert,
        df_dir
    ], axis=1)
    return X, mlb, df_cert, df_dir

# Функция для обучения и оценки моделей регрессии
# - Обучает каждую модель на тренировочных данных
# - Предсказывает значения на тестовой выборке
# - Выводит метрики качества: MAE, RMSE, R2
def evaluate_models(models, X_train, X_test, y_train, y_test):
    print("=== Прогнозирование рейтинга (MAE, RMSE, R2) ===")
    for name, model in models.items():
        model.fit(X_train, y_train)
        y_pred = model.predict(X_test)
        print(f"\nМодель: {name}")
        print('MAE (средняя абсолютная ошибка):', mean_absolute_error(y_test, y_pred))
        print('RMSE (корень из среднеквадратичной ошибки):', np.sqrt(mean_squared_error(y_test, y_pred)))
        print('R2 (коэффициент детерминации):', r2_score(y_test, y_pred))

# Функция для подготовки признаков для нового фильма
# - Кодирует жанры, сертификат и режиссера аналогично обучающей выборке
# - Возвращает датафрейм с признаками для подачи в модель
def prepare_new_movie_features(new_movie, mlb, df_cert, df_dir):
    new_genre = mlb.transform([new_movie['Genre']])
    new_cert = pd.DataFrame([[1 if c == f"Cert_{new_movie['Certificate']}" else 0 for c in df_cert.columns]], columns=df_cert.columns)
    director_val = new_movie['Director'] if f"Dir_{new_movie['Director']}" in df_dir.columns else 'Other'
    new_dir = pd.DataFrame([[1 if d == f"Dir_{director_val}" else 0 for d in df_dir.columns]], columns=df_dir.columns)
    new_X = pd.concat([
        pd.DataFrame(new_genre, columns=mlb.classes_),
        pd.DataFrame([[new_movie['Duration_min'], new_movie['Grossed in $']]], columns=['Duration_min', 'Grossed in $']),
        new_cert,
        new_dir
    ], axis=1)
    return new_X

# === Основной скрипт ===
# 1. Загружаем данные из CSV
# 2. Проводим предобработку и кодирование признаков
# 3. Делим данные на обучающую и тестовую выборки
# 4. Обучаем и оцениваем модели
# 5. Выполняем ассоциативный анализ и кластеризацию
# 6. Пример предсказания для нового фильма

# Загрузка и предобработка данных
raw_df = pd.read_csv('IMDb top 1000 movies.csv')
df, top_directors = preprocess_dataframe(raw_df)
X, mlb, df_cert, df_dir = encode_features(df)
y = df['IMDb Rating']

# Деление на обучающую и тестовую выборки (80/20)
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

# Словарь моделей для сравнения: случайный лес и линейная регрессия
models = {
    'RandomForest': RandomForestRegressor(random_state=42),
    'LinearRegression': LinearRegression()
}
# Оценка моделей
evaluate_models(models, X_train, X_test, y_train, y_test)

# =========================
# ЗАДАЧА 2: Поиск ассоциативных правил между жанрами и рейтингом
# =========================
# Категоризация рейтинга на три группы: низкий, средний, высокий
rating_bins = [0, 7, 8, 10]
rating_labels = ['низкий', 'средний', 'высокий']
df['Rating_cat'] = pd.cut(df['IMDb Rating'], bins=rating_bins, labels=rating_labels, include_lowest=True)

# Формируем бинарную матрицу "транзакций":
# - по жанрам (каждый жанр — отдельный столбец)
# - по категориям рейтинга (отдельные столбцы для каждой категории)
trans = pd.DataFrame(mlb.transform(df['Genre']), columns=mlb.classes_)
for label in rating_labels:
    trans[f'Rating_{label}'] = (df['Rating_cat'] == label).astype(int)
trans = trans.astype(bool)  # Требование для алгоритма apriori

# Поиск часто встречающихся наборов признаков (apriori) и построение ассоциативных правил
frequent = apriori(trans, min_support=0.1, use_colnames=True)
rules = association_rules(frequent, metric="lift", min_threshold=1.0)

print("\n=== Ассоциативные правила ===")
if rules.shape[0] == 0:
    print("Не найдено ассоциативных правил с заданными параметрами.")
else:
    print(rules[['antecedents', 'consequents', 'support', 'confidence', 'lift']].head(10).rename(columns={
        'antecedents': 'Левая часть',
        'consequents': 'Правая часть',
        'support': 'Поддержка',
        'confidence': 'Достоверность',
        'lift': 'Подъём'
    }))

# =========================
# ЗАДАЧА 3: Кластеризация жанров по среднему рейтингу и длительности
# =========================
# Для каждого жанра считаем средний рейтинг и среднюю длительность фильмов
rows = []
for genre in mlb.classes_:
    genre_mask = df['Genre'].apply(lambda genres: genre in genres)
    rows.append({
        'Genre': genre,
        'Mean_Rating': df.loc[genre_mask, 'IMDb Rating'].mean(),
        'Mean_Duration': df.loc[genre_mask, 'Duration_min'].mean()
    })
genre_df = pd.DataFrame(rows).set_index('Genre')

# Масштабируем признаки и понижаем размерность до 2D с помощью PCA
scaler = StandardScaler()
genre_scaled = scaler.fit_transform(genre_df)
pca = PCA(n_components=2)
genre_pca = pca.fit_transform(genre_scaled)

# Кластеризация жанров на 3 группы с помощью KMeans
kmeans = KMeans(n_clusters=3, random_state=42)
clusters = kmeans.fit_predict(genre_pca)

# Визуализация кластеров жанров на плоскости главных компонент
plt.figure(figsize=(8,6))
sns.scatterplot(x=genre_pca[:,0], y=genre_pca[:,1], hue=clusters, palette='Set1')
for i, genre in enumerate(genre_df.index):
    plt.text(genre_pca[i,0], genre_pca[i,1], genre)
plt.title('Кластеризация жанров (PCA + KMeans)')
plt.xlabel('Первая главная компонента (PC1)')
plt.ylabel('Вторая главная компонента (PC2)')
plt.legend(title='Кластер')
plt.show()

# === Пример: прогноз рейтинга для нового фильма ===
# Формируем признаки для нового фильма и делаем предсказание с помощью обученной модели
new_movie = {
    'Genre': ['Drama', 'Crime'],
    'Duration_min': 120,
    'Grossed in $': 100000000,
    'Certificate': 'R',
    'Director': 'Christopher Nolan'
}
new_X = prepare_new_movie_features(new_movie, mlb, df_cert, df_dir)
pred_rating = models['RandomForest'].predict(new_X)[0]
print(f"\nПредсказанный рейтинг IMDb для нового фильма: {pred_rating:.2f}")
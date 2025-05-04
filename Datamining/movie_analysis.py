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

# Загрузка и предобработка данных IMDb top 1000
# Используем только нужные столбцы: Genre, Duration, IMDb Rating

df = pd.read_csv('IMDb top 1000 movies.csv')

# Удаляем строки без рейтинга, жанра или длительности
# Преобразуем жанры в список
# Преобразуем длительность в минуты (число)
df = df.dropna(subset=['Genre', 'IMDb Rating', 'Duration'])
df['Genre'] = df['Genre'].apply(lambda x: [g.strip() for g in str(x).split(',')])
df['Duration_min'] = df['Duration'].str.extract(r'(\d+)').astype(int)

# Добавим дополнительные признаки: Metascore, Votes, Grossed in $, Certificate, Director
# Преобразуем числовые признаки
for col in ['Metascore', 'Votes', 'Grossed in $']:
    df[col] = pd.to_numeric(df[col], errors='coerce')

# Кодируем категориальные признаки: Certificate, Director
# Оставим только топ-10 самых популярных режиссеров, остальные - 'Other'
top_directors = df['Director'].value_counts().nlargest(10).index

# Заполним пропуски в Director и Certificate
for col in ['Director', 'Certificate']:
    df[col] = df[col].fillna('Unknown')
df['Director'] = df['Director'].apply(lambda x: x if x in top_directors else 'Other')

# One-hot для Certificate и Director
df_cert = pd.get_dummies(df['Certificate'], prefix='Cert')
df_dir = pd.get_dummies(df['Director'], prefix='Dir')

# =========================
# ЗАДАЧА 1: Прогнозирование рейтинга
# =========================
X = df[['Genre', 'Duration_min']]
y = df['IMDb Rating']

# Мульти-кодирование жанров
mlb = MultiLabelBinarizer()
X_genre = mlb.fit_transform(X['Genre'])

# Итоговая матрица признаков (без Metascore и Votes)
X = pd.concat([
    pd.DataFrame(X_genre, columns=mlb.classes_),
    df[['Duration_min', 'Grossed in $']].fillna(0),  # Исключаем 'Metascore' и 'Votes'
    df_cert,
    df_dir
], axis=1)

# Разделение
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

models = {
    'RandomForest': RandomForestRegressor(random_state=42),
    'LinearRegression': LinearRegression()
}

print("=== Прогнозирование рейтинга (MAE, RMSE, R2) ===")
for name, model in models.items():
    model.fit(X_train, y_train)
    y_pred = model.predict(X_test)
    print(f"\nМодель: {name}")
    print('MAE (средняя абсолютная ошибка):', mean_absolute_error(y_test, y_pred))
    print('RMSE (корень из среднеквадратичной ошибки):', np.sqrt(mean_squared_error(y_test, y_pred)))
    print('R2 (коэффициент детерминации):', r2_score(y_test, y_pred))

# =========================
# ЗАДАЧА 2: Ассоциативные правила
# =========================
# Категоризация рейтинга
rating_bins = [0, 7, 8, 10]
rating_labels = ['низкий', 'средний', 'высокий']
df['Rating_cat'] = pd.cut(df['IMDb Rating'], bins=rating_bins, labels=rating_labels, include_lowest=True)

# Формируем транзакции: жанры + категория рейтинга
trans = pd.DataFrame(mlb.transform(df['Genre']), columns=mlb.classes_)
for label in rating_labels:
    trans[f'Rating_{label}'] = (df['Rating_cat'] == label).astype(int)
trans = trans.astype(bool)  # Исправление предупреждения: преобразуем к bool

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
# ЗАДАЧА 3: Кластеризация жанров
# =========================
# Средний рейтинг и длительность по жанру
rows = []
for genre in mlb.classes_:
    genre_mask = df['Genre'].apply(lambda genres: genre in genres)
    rows.append({
        'Genre': genre,
        'Mean_Rating': df.loc[genre_mask, 'IMDb Rating'].mean(),
        'Mean_Duration': df.loc[genre_mask, 'Duration_min'].mean()
    })
genre_df = pd.DataFrame(rows).set_index('Genre')

# Масштабирование и PCA
scaler = StandardScaler()
genre_scaled = scaler.fit_transform(genre_df)
pca = PCA(n_components=2)
genre_pca = pca.fit_transform(genre_scaled)

# Кластеризация
kmeans = KMeans(n_clusters=3, random_state=42)
clusters = kmeans.fit_predict(genre_pca)

plt.figure(figsize=(8,6))
sns.scatterplot(x=genre_pca[:,0], y=genre_pca[:,1], hue=clusters, palette='Set1')
for i, genre in enumerate(genre_df.index):
    plt.text(genre_pca[i,0], genre_pca[i,1], genre)
plt.title('Кластеризация жанров (PCA + KMeans)')
plt.xlabel('Первая главная компонента (PC1)')
plt.ylabel('Вторая главная компонента (PC2)')
plt.legend(title='Кластер')
plt.show()

# Пример: предсказание рейтинга для нового фильма
new_movie = {
    'Genre': ['Drama', 'Crime'],
    'Duration_min': 120,
    'Grossed in $': 100000000,  # Исключаем 'Metascore' и 'Votes'
    'Certificate': 'R',
    'Director': 'Christopher Nolan'
}

# Кодирование жанров
new_genre = mlb.transform([new_movie['Genre']])
# One-hot для Certificate и Director
new_cert = pd.DataFrame([[1 if c == f"Cert_{new_movie['Certificate']}" else 0 for c in df_cert.columns]], columns=df_cert.columns)
director_val = new_movie['Director'] if f"Dir_{new_movie['Director']}" in df_dir.columns else 'Other'
new_dir = pd.DataFrame([[1 if d == f"Dir_{director_val}" else 0 for d in df_dir.columns]], columns=df_dir.columns)

# Собираем все признаки (без Metascore и Votes)
new_X = pd.concat([
    pd.DataFrame(new_genre, columns=mlb.classes_),
    pd.DataFrame([[new_movie['Duration_min'], new_movie['Grossed in $']]], columns=['Duration_min', 'Grossed in $']),
    new_cert,
    new_dir
], axis=1)

# Предсказание
pred_rating = models['RandomForest'].predict(new_X)[0]
print(f"\nПредсказанный рейтинг IMDb для нового фильма: {pred_rating:.2f}")
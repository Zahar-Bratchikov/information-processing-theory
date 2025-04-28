"""
Реализация алгоритма сжатия данных LZ77
LZ77 - алгоритм сжатия без потерь, работающий со скользящим окном.
Алгоритм ищет повторяющиеся фрагменты в тексте и заменяет их на ссылки
на ранее встречавшиеся фрагменты.
"""

def find_max_substr(char_number, buffer, source_sequence):
    """
    Функция для поиска наибольшей подпоследовательности в буфере.
    
    Параметры:
        char_number (int): Текущая позиция в исходной последовательности
        buffer (str): Буфер, в котором ищется подпоследовательность
        source_sequence (str): Исходная последовательность символов
        
    Возвращает:
        tuple: Найденная подпоследовательность и новая позиция в исходной последовательности
    """
    result_sequence = source_sequence[char_number]
    if result_sequence in buffer:
        # Расширяем найденную подпоследовательность, пока она содержится в буфере
        while char_number + 1 < len(source_sequence) and result_sequence + source_sequence[char_number + 1] in buffer:
            char_number += 1
            result_sequence += source_sequence[char_number]
        return result_sequence, char_number
    return "", char_number

def get_mon(match_len):
    """
    Генерация унарного кода длины последовательности.
    
    Параметры:
        match_len (int): Длина совпадающей последовательности
        
    Возвращает:
        str: Унарный код для длины последовательности
    """
    if match_len == 0:
        return "0"
    match_len_bin = bin(match_len)[2:]  # Получаем двоичное представление без префикса '0b'
    return '1' * (len(match_len_bin) - 1) + '0' + match_len_bin[1:]  # Формируем унарный код

def is_power_of_two(x):
    """
    Проверка, является ли число степенью двойки.
    
    Параметры:
        x (int): Проверяемое число
        
    Возвращает:
        bool: True, если число является степенью двойки, иначе False
    """
    return (x & (x - 1)) == 0  # Побитовая операция для проверки степени двойки

def get_bin_seq(match_len_symbols_in_buffer, offset):
    """
    Генерация бинарного представления смещения.
    
    Параметры:
        match_len_symbols_in_buffer (int): Количество символов в буфере до совпадения
        offset (int): Смещение до найденного совпадения
        
    Возвращает:
        str: Бинарное представление смещения
    """
    # Определяем длину последовательности для кодирования смещения
    seq_len = len(bin(match_len_symbols_in_buffer)) - 2  # Вычитаем '0b'
    if is_power_of_two(match_len_symbols_in_buffer):
        seq_len -= 1  # Корректируем длину для степеней двойки
    offset_bin = bin(offset)[2:].zfill(seq_len)  # Дополняем нулями слева до нужной длины
    return offset_bin

def algorithm(source_sequence, win_size):
    """
    Основной алгоритм LZ77.
    
    Параметры:
        source_sequence (str): Исходная последовательность для сжатия
        win_size (int): Размер окна буфера
        
    Результат:
        Выводит пошаговое выполнение алгоритма и итоговое количество битов
    """
    char_number = 0  # Текущая позиция в исходной последовательности
    step = 0         # Текущий шаг алгоритма
    buffer = ""      # Буфер (скользящее окно)
    all_bits_count = 0  # Общее количество битов в сжатой последовательности

    # Вывод заголовка таблицы
    print(f"{'Шаг':<5} | {'Флаг':<4} | {'Последовательность букв':<20} | {'d':<10} | {'l':<5} | {'Кодовая последовательность':<25} | {'Биты':<5}")
    print('-' * 90)

    while char_number < len(source_sequence):
        # Ищем наибольшую подпоследовательность в буфере
        match, new_char_number = find_max_substr(char_number, buffer, source_sequence)
        char_number = new_char_number

        if match:
            # Если найдено совпадение
            flag = 1  # Флаг указывает, что используется ссылка
            match_len = len(match)  # Длина совпадения
            offset = char_number - match_len - buffer.index(match)  # Вычисляем смещение
            unar = get_mon(match_len)  # Получаем унарный код для длины
            buffer += match  # Добавляем найденную последовательность в буфер
            match_len_symbols_in_buffer = buffer.rindex(match)  # Находим позицию в буфере
            binary_sequence = get_bin_seq(match_len_symbols_in_buffer, offset)  # Получаем бинарное представление смещения
        else:
            # Если совпадение не найдено
            flag = 0  # Флаг указывает, что используется литерал (буква)
            match = source_sequence[char_number]  # Берем текущий символ
            match_len = 0  # Длина совпадения равна 0
            offset = -1  # Смещение не используется
            match_len_symbols_in_buffer = -1  # Позиция в буфере не используется
            binary_sequence = ""  # Бинарная последовательность пуста
            unar = format(ord(match), '08b')  # Кодируем символ в 8-битную последовательность
            buffer += match  # Добавляем символ в буфер

        # Вычисляем количество битов в текущем коде
        bits_count = 1 + len(binary_sequence) + len(unar)  # Флаг + бинарная последовательность + унарный код
        all_bits_count += bits_count  # Увеличиваем общее количество битов

        # Форматируем вывод для смещения
        offset_str = "-" if offset == -1 else f"{offset}({match_len_symbols_in_buffer})"

        # Выводим информацию о текущем шаге
        print(f"{step:<5} | {flag:<4} | {match:<20} | {offset_str:<10} | {match_len:<5} | {flag} {binary_sequence} {unar:<25} | {bits_count:<5}")

        char_number += 1  # Переходим к следующему символу
        step += 1  # Увеличиваем счетчик шагов

        # Ограничиваем размер буфера
        if len(buffer) > win_size:
            buffer = buffer[-win_size:]  # Оставляем только последние win_size символов

    # Выводим итоговую информацию
    print('-' * 90)
    print(f"Итого битов: {all_bits_count}")

if __name__ == "__main__":
    # Пример использования алгоритма
    source_sequence = "IF_WE_CANNOT_DO_AS_WE_WOULD_WE_SHOULD_DO_AS_WE_CAN"
    win_size = 100  # Размер окна (буфера)
    algorithm(source_sequence, win_size)  # Запускаем алгоритм
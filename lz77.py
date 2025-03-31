def find_max_substr(char_number, buffer, source_sequence):
    """
    Функция для поиска наибольшей подпоследовательности в буфере.
    """
    result_sequence = source_sequence[char_number]
    if result_sequence in buffer:
        while char_number + 1 < len(source_sequence) and result_sequence + source_sequence[char_number + 1] in buffer:
            char_number += 1
            result_sequence += source_sequence[char_number]
        return result_sequence, char_number
    return "", char_number

def get_mon(match_len):
    """
    Генерация унарного кода длины последовательности.
    """
    if match_len == 0:
        return "0"
    match_len_bin = bin(match_len)[2:]
    return '1' * (len(match_len_bin) - 1) + '0' + match_len_bin[1:]

def is_power_of_two(x):
    """
    Проверка, является ли число степенью двойки.
    """
    return (x & (x - 1)) == 0

def get_bin_seq(match_len_symbols_in_buffer, offset):
    """
    Генерация бинарного представления смещения.
    """
    seq_len = len(bin(match_len_symbols_in_buffer)) - 2
    if is_power_of_two(match_len_symbols_in_buffer):
        seq_len -= 1
    offset_bin = bin(offset)[2:].zfill(seq_len)
    return offset_bin

def algorithm(source_sequence, win_size):
    """
    Основной алгоритм.
    """
    char_number = 0
    step = 0
    buffer = ""
    all_bits_count = 0

    print(f"{'Шаг':<5} | {'Флаг':<4} | {'Последовательность букв':<20} | {'d':<10} | {'l':<5} | {'Кодовая последовательность':<25} | {'Биты':<5}")
    print('-' * 90)

    while char_number < len(source_sequence):
        match, new_char_number = find_max_substr(char_number, buffer, source_sequence)
        char_number = new_char_number

        if match:
            flag = 1
            match_len = len(match)
            offset = char_number - match_len - buffer.index(match)
            unar = get_mon(match_len)
            buffer += match
            match_len_symbols_in_buffer = buffer.rindex(match)
            binary_sequence = get_bin_seq(match_len_symbols_in_buffer, offset)
        else:
            flag = 0
            match = source_sequence[char_number]
            match_len = 0
            offset = -1
            match_len_symbols_in_buffer = -1
            binary_sequence = ""
            unar = format(ord(match), '08b')
            buffer += match

        bits_count = 1 + len(binary_sequence) + len(unar)
        all_bits_count += bits_count

        offset_str = "-" if offset == -1 else f"{offset}({match_len_symbols_in_buffer})"

        print(f"{step:<5} | {flag:<4} | {match:<20} | {offset_str:<10} | {match_len:<5} | {flag} {binary_sequence} {unar:<25} | {bits_count:<5}")

        char_number += 1
        step += 1

        if len(buffer) > win_size:
            buffer = buffer[-win_size:]

    print('-' * 90)
    print(f"Итого битов: {all_bits_count}")

if __name__ == "__main__":
    source_sequence = "IF_WE_CANNOT_DO_AS_WE_WOULD_WE_SHOULD_DO_AS_WE_CAN"
    win_size = 100
    algorithm(source_sequence, win_size)
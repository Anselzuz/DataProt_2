using System.Text;

string txt = "Шла Саша по шоссе и сосала сушку.";
string key1 = "Ключ1";
string key2 = "Ключ2";
const string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя_.,!?;:\"'";

Dictionary<char, int> GenerateDictionary(string alphabet)
{
    Dictionary<char, int> alphabetDic = new(alphabet.Length);
    for (int i = 1; i < alphabet.Length; ++i)
        alphabetDic.Add(alphabet[i], i);

    return alphabetDic;
}

int FindMinIndex(int[] mass)
{
    int min = mass[0];
    int minIndex = 0;
    for (int i = 0; i < mass.Length; ++i)
    {
        if (mass[i] < min)
        {
            min = mass[i];
            minIndex = i;
        }
    }

    return minIndex;
}

int[] FindKeyIndexes(Dictionary<char, int> dic, string key)
{
    int[] codesKey = new int[key.Length];
    for (int i = 0; i < key.Length; ++i)
        dic.TryGetValue(key[i], out codesKey[i]);

    int[] indexesKey = new int[key.Length];
    for (int i = 0; i < indexesKey.Length; ++i)
    {
        int minIndex = FindMinIndex(codesKey);
        indexesKey[minIndex] = i;
        codesKey[minIndex] = dic.Count() + 1;
    }

    return indexesKey;
}

int InitialProccesing(ref string key1, ref string key2, string txt)
{
    //Проверка ключей
    if (key1.Length == 0 || key2.Length == 0)
    {
        Console.WriteLine("Ключ не может иметь длину 0.");
        Environment.Exit(-1);
    }

    //Находим количество строк
    int strings;
    if (txt.Length % key1.Length == 0)
        strings = txt.Length / key1.Length;
    else
        strings = txt.Length / key1.Length + 1;

    //Обрезаем / дополняем ключи
    if (strings == 1)
        key1 = key1[..txt.Length];
    if (key2.Length > strings)
        key2 = key2[..strings];
    if (key2.Length < strings)
    {
        string key2Buff = key2;
        int i = strings - key2.Length;
        while (true)
        {
            bool flag = false;
            foreach (var el in key2)
            {
                key2Buff += el;
                if (--i == 0)
                {
                    key2 = key2Buff;
                    flag = true;
                    break;
                }
            }
            if (flag)
                break;
        }
    }

    return strings;
}

string Encrypt(string txt, string key1, string key2)
{
    Dictionary<char, int> dic = GenerateDictionary(alphabet);
    int strings = InitialProccesing(ref key1, ref key2, txt);
    //Генерируем результирующий список
    List<StringBuilder> result = new();
    for (int i = 0; i < strings; ++i)
    {
        string strBuff = new(' ', key1.Length);
        result.Add(new StringBuilder(strBuff, key1.Length));
    }
    //Заполняем его текстом
    int str = 0, col = 0;
    foreach (var el in txt)
    {
        result[str][col] = el;
        ++col;
        if (col == key1.Length)
        {
            ++str;
            col = 0;
        }
    }

    //Определяем позиции букв 1 ключа в алфавите
    int[] indexesKey1 = FindKeyIndexes(dic, key1.ToLower());

    //Применяем ключ 1 к тексту
    for (int i = 0; i < result.Count(); ++i)
    {
        StringBuilder strBuff = new(result[i].ToString());
        for (int j = 0; j < key1.Length; ++j)
            result[i][indexesKey1[j]] = strBuff[j];
    }

    //Определяем позиции букв 2 ключа в алфавите
    int[] indexesKey2 = FindKeyIndexes(dic, key2.ToLower());

    //Применяем ключ 2 к тексту
    List<StringBuilder> buff = new(result);
    for (int i = 0; i < result.Count(); ++i)
        result[indexesKey2[i]] = buff[i];

    //Преобразуем результат в строку
    string stringResult = "";
    foreach (var el in result)
        stringResult += el;

    return stringResult;
}

string Decrypt(string txt, string key1, string key2)
{
    Dictionary<char, int> dic = GenerateDictionary(alphabet);
    int strings = InitialProccesing(ref key1, ref key2, txt);
    //Генерируем результирующий список
    List<StringBuilder> result = new();
    for (int i = 0; i < strings; ++i)
    {
        string strBuff = new(' ', key1.Length);
        result.Add(new StringBuilder(strBuff, key1.Length));
    }
    //Заполняем его текстом
    int str = 0, col = 0;
    foreach (var el in txt)
    {
        result[str][col] = el;
        ++col;
        if (col == key1.Length)
        {
            ++str;
            col = 0;
        }
    }

    //Определяем позиции букв 2 ключа в алфавите
    int[] indexesKey2 = FindKeyIndexes(dic, key2.ToLower());

    //Применяем 2 ключ
    List<StringBuilder> buff = new(result);
    for (int i = 0; i < result.Count(); ++i)
        result[i] = buff[indexesKey2[i]];

    //Определяем позиции букв 1 ключа в алфавите
    int[] indexesKey1 = FindKeyIndexes(dic, key1.ToLower());

    //Применяем 1 ключ
    for (int i = 0; i < result.Count(); ++i)
    {
        StringBuilder strBuff = new(result[i].ToString());
        for (int j = 0; j < key1.Length; ++j)
            result[i][j] = strBuff[indexesKey1[j]];
    }

    //Преобразуем результат в строку
    string stringResult = "";
    foreach (var el in result)
        stringResult += el;

    return stringResult;
}

Console.WriteLine("Исходный текст:\t\t" + txt);
string encryptText = Encrypt(txt, key1, key2);
Console.WriteLine("Закодированный текст:\t" + encryptText);
string decryptText = Decrypt(encryptText, key1, key2);
Console.WriteLine("Расшифрованный текст:\t" + decryptText);
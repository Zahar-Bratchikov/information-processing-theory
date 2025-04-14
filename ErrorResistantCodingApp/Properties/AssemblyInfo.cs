// Файл AssemblyInfo.cs содержит атрибуты сборки, которые определяют метаданные .NET сборки
// Эти атрибуты используются .NET Framework и Visual Studio для различных целей

using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

// Общие сведения об этой сборке предоставляются следующим набором
// набор атрибутов. Измените значения этих атрибутов, чтобы изменить сведения,
// связанные со сборкой.
[assembly: AssemblyTitle("ErrorResistantCodingApp")] // Название приложения
[assembly: AssemblyDescription("")] // Описание приложения
[assembly: AssemblyConfiguration("")] // Конфигурация сборки (Debug/Release и т.д.)
[assembly: AssemblyCompany("HP")] // Название компании
[assembly: AssemblyProduct("ErrorResistantCodingApp")] // Название продукта
[assembly: AssemblyCopyright("Copyright © HP 2025")] // Информация об авторских правах
[assembly: AssemblyTrademark("")] // Информация о торговых марках
[assembly: AssemblyCulture("")] // Культурные настройки

// Установка значения False для параметра ComVisible делает типы в этой сборке невидимыми
// для компонентов COM. Если необходимо обратиться к типу в этой сборке через
// из модели COM, установите атрибут ComVisible для этого типа в значение true.
[assembly: ComVisible(false)] // Контролирует видимость типов для COM-объектов

//Чтобы начать создание локализуемых приложений, задайте
//<UICulture>CultureYouAreCodingWith</UICulture> в файле .csproj
//в <PropertyGroup>. Например, при использовании английского (США)
//в своих исходных файлах установите <UICulture> в en-US.  Затем отмените преобразование в комментарий
//атрибута NeutralResourceLanguage ниже.  Обновите "en-US" в
//строка внизу для обеспечения соответствия настройки UICulture в файле проекта.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]

// Атрибут ThemeInfo описывает расположение тематических словарей ресурсов для WPF приложения
[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //где расположены словари ресурсов по конкретным тематикам
                                     //(используется, если ресурс не найден на странице,
                                     // или в словарях ресурсов приложения)
    ResourceDictionaryLocation.SourceAssembly //где расположен словарь универсальных ресурсов
                                              //(используется, если ресурс не найден на странице,
                                              // в приложении или в каких-либо словарях ресурсов для конкретной темы)
)]


// Сведения о версии для сборки включают четыре следующих значения:
//
//      Основной номер версии - номер основной версии продукта (major)
//      Дополнительный номер версии - номер дополнительной версии продукта (minor)
//      Номер сборки - номер билда/сборки (build)
//      Номер редакции - номер ревизии (revision)
//
[assembly: AssemblyVersion("1.0.0.0")] // Версия сборки, используемая CLR для поиска сборки
[assembly: AssemblyFileVersion("1.0.0.0")] // Версия файла, используемая для отображения в Windows

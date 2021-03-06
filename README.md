# City-Roads
Визуализация графа дорог Волгограда и построение списка и матрицы смежности Волгограда  

Белоусов Юрий Вячеславович
332 группа

> TL;DR Описание алгоритма доступно [здесь](docs/algorithm.md), результат для Волгограда вот [здесь](docs/volgograd.md)

* [Состояние сборки](#состояние-сборки)
* [Задание](#задание)
* [Функционал и алгоритм](#функционал)
* [Инструкция по запуску](#инструкция-по-запуску)
* [Результат работы программы](#результат-работы-программы)
* [Благодарности](#благодарности)

___

# Состояние сборки
* __master branch__ [![Build status](https://ci.appveyor.com/api/projects/status/o8jhw537h3kihxxk/branch/master?svg=true)](https://ci.appveyor.com/project/bruce-willis/city-roads/branch/master)

* __develop branch__ [![Build status](https://ci.appveyor.com/api/projects/status/o8jhw537h3kihxxk/branch/develop?svg=true)](https://ci.appveyor.com/project/bruce-willis/city-roads/branch/develop)

Скачать последнюю стабильную версию (сборка из ветви `master`) под вашу платформу возможно [отсюда](https://ci.appveyor.com/project/bruce-willis/city-roads/branch/master/artifacts)

# Задание
С полным текстом задания можно ознакомиться [здесь](docs/task.md)

# Функционал
* Построение визуализации графа
* Построение информации про вершины
* Построение списка смежности
* Построение матрицы смежности
* Случайным образом отвечает на один из двух самых важных вопроса в жизни

__Информация об алгоритме__, используемых технологиях доступна [здесь](docs/algorithm.md)

# Инструкция по запуску
Проверить на соответствие [системным требованиям](https://github.com/dotnet/core/blob/master/release-notes/2.0/2.0-supported-os.md)
* Использовать готовую сборку
    * скачать и распаковать сборку для вашей ОС [отсюда](https://github.com/bruce-willis/City-Roads/releases/tag/0.1)
    * положить в директорию с билдом файл `*.osm` с нужным участком карты. ([Инструкция по скачиванию](docs/download.md) произвольного участка, либо можно скачать файл для Волгограда, который также находится на странице со сборками)
    * открыть командную строку
    * выполнить следующую команду: `.\CityMap -f filename.osm`, где `filename.osm` — название карты. 
* Собрать из исходников
    * установить [.NET Core SDK](https://www.microsoft.com/net/download/) для вашей ОС
    * скачать репозиторий (`git clone https://github.com/bruce-willis/City-Roads.git` или просто архивом)
    * Зайти в директорию `City-Roads/src` (`cd .\City-Roads\src`)
    * добавить файл с картой
    * открыть командую строку
    * выполнить следующую команду: `dotnet run -- -f filename.osm`
    
По умолчанию программа ищет файл с названием `map.osm`, так что если у вас называется точно такое же, то можете не указывать дополнительные аргументы

Если вы хотите, то можете включить генерацию матрицы смежности (__внимание:__ данный процесс крайне небыстрый (не менее 20 минут) и получившийся файл может весить более 30Гб) с помощью аргумента `-m` (`--write-matrix`)

Узнать обо всех возможностях программы можно с помощью аргумента `--help`

# Результат работы программы

По умолчанию программа создаст директорию `Output`, в которой будут сгенерированы 3 файла
* `map.svg` - файл с векторной визуализацией построенного графа
* `nodes_list.csv` - таблица с информацией о вершинах
* `adjacency_list.csv` - таблица со списком смежности


__Результаты для Волгограда__ вместе с подробным описанием доступны [здесь](docs/volgograd.md)

# Благодарности
* [Appveyor](https://www.appveyor.com/) за возможность бесплатной настройки непрерывной интеграции и классный сервис
* [Resharper](https://www.jetbrains.com/resharper/) за чрезвычайно полезное дополнение, которое (я надеюсь) сделало код чуточку лучше и понятней
* Всем тем, кто воспользовался и указал ссылку на инструкцию по скачиванию географической области. Мне приятно, спасибо!
* [Андрею](https://github.com/vahriin) и [Сереже](https://github.com/Piteryo) за тестирование сборок
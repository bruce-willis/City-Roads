# Алгоритм и используемые технологии

___

* [Алгоритм и используемые технологии](#алгоритм-и-используемые-технологии)
* [Язык разработки](#язык-разработки)
	* [Использованные библиотеки:](#использованные-библиотеки)
* [Описание алгоритма](#описание-алгоритма)

___

# Язык разработки
В качестве языка выбран `c#`, а точнее `.net core`, что позволило получить бинарники, которые можно запустить на всех популярных операционных системах.

## Использованные библиотеки:
* [commandline](https://github.com/commandlineparser/commandline) - парсер аргументов командной строки

# Описание алгоритма

* Парсинг исходного файлы в [классы](https://github.com/bruce-willis/City-Roads/tree/develop/src/Types)  
```cs
using (var reader = new StreamReader(options.FileName))
{
    city = (City) new XmlSerializer(typeof(City)).Deserialize(reader);
}
```
* Если в исходном файле не было информации про наименьшую и наибольшую границу, то находим их самостоятельно
* Создание ассоциативного массива, где в качестве ключа используется `id` точки, а в качестве значение класс [`GeoPoint`](https://github.com/bruce-willis/City-Roads/blob/develop/src/Types/GeoPoint.cs), которых хранит географические координаты и смежные вершины  
```cs
Dictionary = city.Nodes.ToDictionary(n => n.Id, n => new GeoPoint(n.Longitude, n.Latitude));
```
* Отбираем только те линии, по которым возможно автомобильное движение, то есть содержат ключ `highway` в тегах и одно из допустимых значений:
```cs
public static readonly Dictionary<string, (string color, double width)> AcceptedHighways = new Dictionary<string, (string color, double width)>
{
    ["motorway"] = ("palevioletred", 1.2),
    ["motorway_link"] = ("palevioletred", 1.2),

    ["trunk"] = ("chocolate", 1.4),
    ["trunk_link"] = ("chocolate", 1.4),

    ["primary"] = ("lightsalmon", 1.2),
    ["primary_link"] = ("lightsalmon", 1.2),

    ["secondary"] = ("indianred", 0.8),
    ["secondary_link"] = ("indianred", 0.8),

    ["tertiary"] = ("darkred", 0.1),
    ["tertiary_link"] = ("darkred", 0.1),

    ["unclassified"] = ("darkred", 0.1),
    ["residential"] = ("darkred", 0.1),
    ["service"] = ("darkred", 0.1),
    ["living_street"] = ("darkred", 0.1),
    ["road"] = ("darkred", 0.1)
};
```
Дополнительно для каждого типа задаем свой цвет и толщину
* Записываем в `svg` используя [`polylyne`](https://developer.mozilla.org/en-US/docs/Web/SVG/Element/polyline), указывая точки, через которые проходят каждая линия
* Оставляем в ассоциативном массиве только те точки, которые используются
```cs
Dictionary = Dictionary.Where(n => n.Value.Used).ToDictionary(n => n.Key, n => n.Value);
```
* Для оставленных точек записываем информацию о них в файл `nodes_list.csv`
* Создаем список смежности

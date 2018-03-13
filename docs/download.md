# Скачивание карты города

- ### Способ 1 (самый простой, но не самый аккуратный) - скачать прямоугольную область:
  - зайти на главную [страницу](https://www.openstreetmap.org/export#map=9/59.9667/30.4184)
  - нажать `выделить другую область`
  - выделить нужный город
  - нажать `скачать с Overpass API`
  - добавить расширение `.osm` для скачанного файла

Проблема — очень редко можно подобрать подходящий прямоугольник для границы города. Может попасть очень много лишнего, например:
![bounds](resources/bounds.jpg "Ну очень много лишнего")

- ### Способ 2 (аккуратный и точный, но требует времени и не такой простой) - скачать данные только для города:
  - Узнать `Relation ID`:
    - зайти на главную [страницу](https://www.openstreetmap.org/export#map=9/59.9667/30.4184)
    - в поиске вбить название города
    - выбрать нужный вариант из списка
    - убедиться по карте, что выбран верный город
    - `Relation ID` находится в левом верхнем углу:
    ![relation](resources/relation_id.png "Relation ID")
  - Получить `полигон` для города:
    - зайти на [сайт](http://polygons.openstreetmap.fr/)
    - ввести `Relation ID` из предыдущего пункта
    ![polygon](https://raw.githubusercontent.com/JamesChevalier/cities/master/images/howto5.png)
    - и нажать `Submit`
    - Выбрать `poly` из предложенных форматов
    ![poly](https://raw.githubusercontent.com/JamesChevalier/cities/master/images/howto6.png)
    - Скопировать все (`Ctrl+A`, `Ctrl+C`), создать файл `<название города>.poly` (например `volgograd.poly`)
    - Пример файла `*.poly` можно посмотреть [`здесь`](../data/volgograd.poly)
  - Скачать карту региона (есть как минимум два варианта)
    - Скачать карту субрегиона:
      - зайти на [сайт](https://download.geofabrik.de/russia.html)
      - выбрать регион, в котором расположен город (подсказка: если вы не уверены в регионе, то по нажатию на регион в новой странице откроется карта с выделенным регионом, где вы можете проверить принадлежит ли данному региону ваш город)
      - скачать в формате `*.osm.pbf`
     ![osm](resources/osm.png)
    - Скачать карту области (только для России)
      - зайти на [сайт](http://be.gis-lab.info/data/osm_dump/dump/latest/)
      - выбрать область, в которой расположен город (подсказка: если вы не уверены/не знаете, как именно расшифровывается аббревиатура области, то уточнить можно вот [здесь](https://github.com/nextgis/osmdump_poly), выбрав нужный полигон)
      - скачать в формате `*.osm.pbf`
      ![osm](resources/osm.pbf.png)
  - Извлечь из файла с форматом `*.osm.pbf` данные только для вашего города
    - С помощью [**Osmosis**](https://wiki.openstreetmap.org/wiki/Osmosis)
      - Установить [`Osmosis`](https://wiki.openstreetmap.org/wiki/Osmosis). Инструкция для [Linux](https://wiki.openstreetmap.org/wiki/Osmosis/Installation#Linux), [Mac OS](https://wiki.openstreetmap.org/wiki/Osmosis/Installation#OS_X), [Windows](https://wiki.openstreetmap.org/wiki/Osmosis/Quick_Install_(Windows))
      - Получить карту только для города. (Для `Windows` проще всего перенести полигон и карту региона в папку `osmosis-latest\bin`). И с помощью командой строки запустить следующую команду:
      `osmosis --read-pbf-fast file="YOUR-REGION-latest.osm.pbf" --bounding-polygon file="CITY-NAME_STATE.poly" --write-xml file="CITY-NAME_STATE.osm"`.
      Например: `osmosis --read-pbf-fast file="south-fed-district-latest.osm.pbf" --bounding-polygon file="volgograd.poly" --write-xml file="volgograd.osm"`
      - Дождаться выполнения команды
      - Необходимый файл будет иметь расширение `.osm`
    - С помощью [**Pbftoosm**](https://wiki.openstreetmap.org/wiki/Pbftoosm)
      - [Скачать с официального сайта](https://wiki.openstreetmap.org/wiki/Pbftoosm#Download)
      - С помощью командой строки запустить следующую команду (для Windows необходимо использовать `cmd`, `Powershell` здесь не будет работать):
      `./pbftoosm < YOUR-REGION-latest.osm.pbf -B=CITY-NAME.poly > CITY-NAME.osm`.
      Например: `./pbftoosm < RU-VGG.osm.pbf -B=volgograd.poly > VGG.osm`
      - *Замечание*. Если вам не нужны теги с историей (version, changeset, user, timestamp), то их можно убрать с помощью аргумента `--drop-history`
      - *Замечание*. Аналогично можно удалить битые ссылки и теги с отношениями, если они вам не нужны, с помощью аргументов `--drop-brokenrefs` и `--drop-relations` соответственно
      - *Замечание*. Иногда в получившимся файле могут остаться ссылки на точки, которые находятся за пределами области, и, как следствие, не попадут в итоговый файл. Чтобы этого избежать необходимо добавить параметр `-i` перед указанием области, то есть `-i=YOUR-REGION-latest.osm.pbf`
      - *Примечание*. При условии выполнения всех замечаний получившийся файл может солидно уменьшится в размере, например при выполнении команды вида `./pbftoosm -i=RU-VGG.osm.pbf -B=volgograd.poly > VGG.osm --drop-history --drop-brokenrefs --drop-relations` итоговый файл в два раза меньше по размеру - 81Мб против 167Мб
    - С помощью [**Osmconvert**](https://wiki.openstreetmap.org/wiki/RU:Osmconvert)

___

## Примечания
- Независимо от выбранного Вами способа, формат полученного документа будет идентичен. Разница лишь в области, которая попадет в файл
- Спасибо за второй пункт [JamesChevalier](https://github.com/JamesChevalier/cities) и интернетам

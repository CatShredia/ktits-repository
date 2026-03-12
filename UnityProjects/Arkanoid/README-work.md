# Arkanoid

### Objects

##### First Level:

- **platform** элемент, управление которым под игроком
  управление на `a` и `d`;
- **ball** передвигается по карте, не может пересечь любой край кроме нижнего
  если пересечет нижний: -1HP;
- **block** спавниваться на карте, при соприкосновении с `ball` пропадает
- _animation_menues_ после поражения или окончания левела - выпадание соответсвующего меню;
- _3 levels_ 3 левела с возрастающей сложностью;

##### Second Level:

- **bonus** после разбития `block` с вероятностью могут выпасть бонусы:

  > Множество шариков: Разделяет текущий шарик на 2-3 дополнительных.

  > Ускорение шарика: Увеличивает скорость движения шарика.

  > Замедление шарика: Уменьшает скорость движения шарика.

  > Расширение платформы: Увеличивает размер платформы.

  > Сжатие платформы: Уменьшает размер платформы;

- _разница между уровнями blocks_ - разное количество HP;

  > Неразрушимые блоки: Блоки, которые нельзя уничтожить.

  > Движущиеся блоки: Блоки, которые перемещаются по экрану, усложняя игру.

  > Отражающие блоки: Меняют направление шарика при столкновении.

##### Third Level:

- _различные звуки_;
- _генерация левелов по паттернам_.

First, Second Levels are required!

​Просканируй скрипты и сделай следующие вещи:

- удали комментарии (кроме тех, что перед public class)
- удали все Debug.Log()
- оптимизируй код где возможно
  Перед участками, которые ты поменяешь в последнем пункте поставь TODO.

---

Я добавил на сцену следующие объекты:

MusicPanel - Canvas
имееет MusicPlayerUIController
В него входят:
MusicImage - изображение обложки
TrackTitle - TextMeshPro Text
last-track-btn - TextMeshPro Button
second-track-btn - TextMeshPro Button
pause-track-btn - TextMeshPro Button
Сделай мне скрипт/скрипты, которые будут вначале включать первую песню, а потом следующие. Кнопки тоже должны работать в соответсвии с названиями.

​Треки находятся:Arkanoid\Assets\Images\Music
Directory: C:\directory-git\ktits-repository-1\UnityProjects\Arkanoid\Assets\Images\Music

Mode LastWriteTime Length Name

---

-a--- 3/12/2026 12:24 AM 314653 1track - Arkanoid Sound.jpg
-a--- 3/12/2026 9:39 AM 3228 1track - Arkanoid Sound.jpg.meta
-a--- 3/12/2026 12:20 AM 7199391 1track - Arkanoid Sound.mp3
-a--- 3/12/2026 9:39 AM 485 1track - Arkanoid Sound.mp3.meta
-a--- 3/12/2026 12:25 AM 368773 2track - Arkanoid Sound.jpg
-a--- 3/12/2026 9:39 AM 3228 2track - Arkanoid Sound.jpg.meta
-a--- 3/12/2026 12:24 AM 7199391 2track - Arkanoid Sound.mp3
-a--- 3/12/2026 9:39 AM 485 2track - Arkanoid Sound.mp3.meta
-a--- 3/12/2026 12:26 AM 136388 3track - Arkanoid Sound.jpg
-a--- 3/12/2026 9:39 AM 3231 3track - Arkanoid Sound.jpg.meta
-a--- 3/12/2026 12:27 AM 7199391 3track - Arkanoid Sound.mp3
-a--- 3/12/2026 9:39 AM 485 3track - Arkanoid Sound.mp3.meta
Сделай сначала новый план выполнения.

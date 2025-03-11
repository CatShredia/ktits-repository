// Находим все элементы с классом mycontainer
const containers = document.querySelectorAll(".mycontainer");

// Создаем Intersection Observer
const observer = new IntersectionObserver(
  (entries) => {
    entries.forEach((entry) => {
      if (entry.isIntersecting) {
        // Добавляем класс visible, когда элемент появляется в области просмотра
        entry.target.classList.add("visible");
      } else {
        // Убираем класс visible, если элемент исчезает из области просмотра
        entry.target.classList.remove("visible");
      }
    });
  },
  {
    root: null, // Используем область просмотра как корень
    rootMargin: "0px", // Без отступов
    threshold: 0.3, // Срабатывает, когда 30% элемента видно
  }
);

// Начинаем отслеживать каждый элемент
containers.forEach((container) => {
  observer.observe(container);
});

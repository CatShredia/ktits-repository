// dependences adding

// добавляем отображение зависимостей, в style самой страницы
let styleElem = $("style");
if (styleElem.length === 0) {
  styleElem = $("<style>");
  styleElem.text(
    `body::before { content: "Dependences: bootstrap jquery fontawesome"; }`
  );
  $("head").append(styleElem);
} else {
  styleElem.text(
    styleElem.text() +
      `body::before { content: "Dependences: bootstrap jquery fontawesome"; }`
  );
}

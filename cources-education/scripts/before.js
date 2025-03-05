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
console.log(styleElem);

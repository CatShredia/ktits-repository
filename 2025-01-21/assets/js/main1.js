const buttonElements = document.querySelectorAll(".button");

buttonElements[0].addEventListener("click", () => {
  console.log("Нажатие");
});
buttonElements[1].addEventListener("mouseover", () => {
  console.log("Мышка зашла");
});
buttonElements[2].addEventListener("mouseout", () => {
  console.log("Мышка вышла");
});
buttonElements[3].addEventListener("keyup", () => {
  console.log("Нажата кнопка up");
});
buttonElements[4].addEventListener("keydown", () => {
  console.log("Нажата кнопка down");
});
buttonElements[5].addEventListener("focus", () => {
  console.log("Фокус");
});
buttonElements[6].addEventListener("blur", () => {
  console.log("Раз Фокус");
});

document.addEventListener("keyup", (event) => {
  if ("a" == event.key.toLowerCase()) {
    console.log("Нажата a!");
  }
  if ("b" == event.key.toLowerCase()) {
    console.log("Нажата b!");
  }
});

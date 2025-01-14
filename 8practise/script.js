const button1 = document.createElement("button");
button1.textContent = "Нажмите для эффекта нажатия";

let i = 0;
button1.onclick = function () {
  if (i == 0) {
    button1.style.color = "white";
    button1.style.backgroundColor = "black";
    button1.style.border = "5px solid yellow";

    i = 1;
  } else {
    button1.style.color = "black";
    button1.style.backgroundColor = "white";
    button1.style.border = "1px solid black";

    i = 0;
  }
};
document.body.appendChild(button1);

const button2 = document.createElement("button");
button2.textContent = "Наведите для эффекта наведения";
button2.onmouseover = function () {
  button2.style.backgroundColor = "blue";
};
button2.onmouseout = function () {
  button2.style.backgroundColor = "white";
};
document.body.appendChild(button2);

const button3 = document.createElement("button");
button3.textContent = "Кликните правой кнопкой мыши для контекстного меню";
button3.oncontextmenu = function () {
  button3.style.backgroundColor = "yellow";
  return false;
};
document.body.appendChild(button3);

let num = 0;
let menuActive = document.querySelector(".menu-active0");
let menuButton = document.querySelector(".menu-button");

function menu() {
  if (num == 0) {
    console.log("Открытие меню");

    menuActive.style.display = "flex";
    menuButton.src = "rec/img/Close.png";

    num++;
  } else {
    console.log("Закрытие меню");

    menuActive.style.display = "none";
    menuButton.src = "rec/img/Menu.png";

    num--;
  }
}

let num = 0;
let menuActive = document.querySelector(".menu-active0");
let menuButton = document.querySelectorAll(".menu-button");

function menu() {
  if (num == 0) {
    console.log("Открытие меню");

    menuActive.style.display = "flex";
    menuButton[0].src = "rec/img/Close.png";

    num++;
  } else {
    console.log("Закрытие меню");

    menuActive.style.display = "none";
    menuButton[0].src = "rec/img/Menu.png";

    num--;
  }
}
const menuFooter = document.querySelector(".menu-footer");

console.log(menuFooter);

function menu2() {
  if (num == 0) {
    console.log("Открытие меню");

    menuFooter.style.display = "flex";
    menuButton[1].src = "rec/img/Close.png";

    num++;
  } else {
    console.log("Закрытие меню");

    menuFooter.style.display = "none";
    menuButton[1].src = "rec/img/Menu.png";

    num--;
  }
}

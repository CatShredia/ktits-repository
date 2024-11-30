let num = 0;
let menuActive = document.querySelector(".menu-active");
function menu() {
  if (num == 0) {
    console.log("Открытие меню");

    menuActive.style.display = "flex";

    num++;
  } else {
    console.log("Закрытие меню");

    menuActive.style.display = "none";

    num--;
  }
}

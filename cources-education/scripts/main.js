console.log("hi");

let menu = document.querySelector(".navmenu");

function menuOpen(elem) {
  console.log(menu);

  menu.style.display = "grid";
}

function menuClose(elem) {
  console.log(menu);

  menu.style.display = "none";
}

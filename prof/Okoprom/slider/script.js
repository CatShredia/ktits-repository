class SliderClass {
  //конструктор слайдера
  constructor(sliderObject, countSlid) {
    console.log("Объект Слайдера");
    this.sliderObject = sliderObject;
    console.log(sliderObject);
    console.log("Количество слайдов на странице");
    this.countSlid = countSlid;
    console.log(countSlid);

    this.displaySliders();

    this.buttonLeft = sliderObject.children[0];
    this.buttonRight = sliderObject.children[countSlid + 1];
  }
  switchLeft() {
    console.log("Левая");
  }
  switchRight() {
    console.log("Правая");
  }
  //отображение слайдов
  displaySliders() {
    for (let i = 0; i <= this.countSlid; i++) {
      console.log(this.sliderObject.children[i]);
      this.sliderObject.children[i].style.display = "flex";
    }
  }
}

function domLoaded() {
  console.log("DOM loaded");

  //sliderObject - объект слайдера
  //countSlid - количество слайдов на странице
  let slider = new SliderClass(document.querySelector(".slider"), 3);
}
addEventListener("DOMContentLoaded", domLoaded());

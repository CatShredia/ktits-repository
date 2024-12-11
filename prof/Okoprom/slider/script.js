class Slider {
  constructor(countSliderOnPage) {
    this.countSliderOnPage = countSliderOnPage; //count of slid on page
    this.slider = document.querySelector(".slider"); //slider object
    this.slids = document.querySelectorAll(".slid"); //collection of slides
    this.countHtmlSlides = this.slids.length; //count slides in html document

    this.start = 0; //display start
    this.stop = countSliderOnPage; //display stop

    this.displaySlids(this.start, this.stop); //first display slide
    this.addEventButton(); //events

    // TODO: for devops
    this.devMode = false; //dev mode
  }

  //TODO: to diplay slides
  displaySlids(start, stop) {
    if (this.devMode) {
      console.log("start: " + start);
      console.log("stop: " + stop);
    }

    for (let i = 0; i < this.countHtmlSlides; i++) {
      this.slids[i].style.display = "none";
    }
    for (let i = start; i < stop; i++) {
      this.slids[i].style.display = "grid";
    }
  }
  addEventButton() {
    const buttons = this.slider.querySelectorAll(".slider-button");

    buttons[0].addEventListener("click", (ivent) => {
      this.switchLeft();
    });

    buttons[1].addEventListener("click", (ivent) => {
      this.switchRight();
    });
  }
  switchLeft() {
    // console.log("left");

    this.start -= 1;
    this.stop -= 1;

    if (this.start < 0) {
      this.start = this.countHtmlSlides - 3;
      this.stop = this.countHtmlSlides;
    }

    this.displaySlids(this.start, this.stop);
  }
  switchRight() {
    // console.log("right");

    this.start += 1;
    this.stop += 1;

    if (this.stop > this.countHtmlSlides) {
      this.stop = 3;
      this.start = 0;
    }

    this.displaySlids(this.start, this.stop);
  }
}

document.addEventListener("DOMContentLoaded", (elem) => {
  let sliderObject = new Slider(3);

  // timer;
  setInterval(function timerSlider() {
    sliderObject.switchRight();
  }, 6000);
});

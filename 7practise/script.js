function task1() {
  const buttonObject = document.getElementById("button");

  console.log(buttonObject);

  buttonObject.style.display = "none";
}
let index = 0;
function task2() {
  const buttonObject2 = document.getElementById("text");
  if (index == 0) {
    buttonObject2.style.display = "none";

    index++;
  } else if (index == 1) {
    buttonObject2.style.display = "block";

    index--;
  }
}

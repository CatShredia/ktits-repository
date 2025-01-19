let header = document.getElementById("headerSelection");

let ccc = 0;

function button1() {
  console.log("1 кнопка");
  header.insertAdjacentHTML(
    "beforebegin",
    "<section class='header-section-el'>ddd</section>"
  );
}

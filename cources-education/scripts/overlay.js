// overlay menu
document.addEventListener("DOMContentLoaded", function () {
  const overlay = document.getElementById("overlay");
  const closeButton = document.getElementById("close-button");
  const popup = document.getElementById("popup");

  function showPopup() {
    overlay.style.opacity = 0;
    overlay.style.display = "flex";
    popup.style.transform = "translateY(-50px)";

    overlay.offsetHeight;

    overlay.style.transition = "opacity 0.3s ease-in-out";
    popup.style.transition = "transform 0.3s ease-in-out";
    overlay.style.opacity = 1;
    popup.style.transform = "translateY(0)";
  }

  function hidePopup() {
    overlay.style.opacity = 0;
    popup.style.transform = "translateY(-50px)";

    overlay.addEventListener(
      "transitionend",
      () => {
        overlay.style.display = "none";
        overlay.style.transition = "none";
        popup.style.transition = "none";
        popup.style.transform = "translateY(0)";
      },
      {
        once: true,
      }
    );
  }

  setTimeout(showPopup, 3000); // 3000 миллисекунд = 3 секунды

  closeButton.addEventListener("click", hidePopup);

  overlay.addEventListener("click", function (event) {
    if (event.target === overlay) {
      hidePopup();
    }
  });
});

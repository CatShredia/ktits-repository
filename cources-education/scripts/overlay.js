$(document).ready(function () {
  const $overlay = $("#overlay");
  const $closeButton = $("#close-button");
  const $popup = $("#popup");

  function showPopup() {
    $overlay.css({
      opacity: 0,
      display: "flex",
    });
    $popup.css("transform", "translateY(-50px)");

    // Force a reflow to ensure the transition starts correctly
    $overlay[0].offsetHeight;

    $overlay.css({
      transition: "opacity 0.3s ease-in-out",
      opacity: 1,
    });
    $popup.css({
      transition: "transform 0.3s ease-in-out",
      transform: "translateY(0)",
    });
  }

  function hidePopup() {
    $overlay.css("opacity", 0);
    $popup.css("transform", "translateY(-50px)");

    $overlay.one("transitionend", function () {
      $overlay.css("display", "none");
      $overlay.css("transition", "none");
      $popup.css("transition", "none");
      $popup.css("transform", "translateY(0)");
    });
  }

  setTimeout(showPopup, 5000);

  $closeButton.on("click", hidePopup);

  $overlay.on("click", function (event) {
    if (event.target === this) {
      hidePopup();
    }
  });
});

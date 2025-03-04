$(document).ready(function () {
  let $menu = $(".navmenu");
  function menuOpen() {
    $menu.addClass("show");
  }
  function menuClose() {
    $menu.removeClass("show");
  }
  $(".icon__link").on("click", function () {
    menuOpen();
  });
  $(".navmenu__close").on("click", function () {
    menuClose();
  });
});
$(document).ready(function () {
  let $menu = $(".navmenu");
  function menuOpen() {
    $menu.addClass("show");
  }
  function menuClose() {
    $menu.removeClass("show");
  }
  $(".icon__link").on("click", function () {
    menuOpen();
  });
  $(".navmenu__close").on("click", function () {
    menuClose();
  });
  // Обработчик для кнопки отзыва
  $(".feedback__button").on("click", function () {
    $(this).closest(".feedback").find(".feedback-content").slideToggle(300); // Плавное открытие/закрытие контента отзыва
  });
});

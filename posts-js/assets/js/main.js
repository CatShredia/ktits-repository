// ? Добавление постов
const formPostsContainer = document.querySelector(".form__inner");
const formPosts = formPostsContainer.children[0].querySelectorAll("input");

const basePost = document.querySelector(".post").cloneNode(true);

const errorElement = document.querySelector(".error");

formPostsContainer.addEventListener("click", (event) => {
  if (event.target.id == "post-form-button") {
    console.log(formPosts[0]);
    let titleValue = formPosts[0].value;
    let authorValue = formPosts[1].value;
    let contentValue = formPosts[2].value;

    // validation
    if (titleValue == "" || authorValue == "" || contentValue == "") {
      errorElement.style.display = "block";
      errorElement.innerHTML = "Вы не заполнили все поля!";
    } else {
      if (titleValue.length < 8) {
        errorElement.style.display = "block";
        errorElement.innerHTML =
          "Количество символов в названии должно быть больше 8";
      } else {
        if (contentValue.length > 300) {
          errorElement.style.display = "block";
          errorElement.innerHTML =
            "Количество символов в контенте не должно превышать 300!";
        } else {
          let newPost = basePost;
          newPost.children[0].innerHTML = titleValue;
          newPost.children[1].innerHTML = authorValue;
          newPost.children[2].innerHTML = contentValue;

          let posts = document.querySelector(".posts-container");
          posts.insertBefore(newPost, posts.lastChild);

          errorElement.style.display = "none";
          errorElement.innerHTML += "";
        }
      }
    }
  }
});

// ? Разные темы

const themeElements = document.querySelectorAll(".theme");

console.log(themeElements);

document.addEventListener("keypress", (event) => {
  if (event.key.toLowerCase() == "w") {
    console.log("White Theme");

    themeElements.forEach((element) => {
      console.log(element);

      element.style.backgroundColor = "white";
      element.style.color = "black";
    });

    document.querySelectorAll("a").forEach((a) => {
      a.style.color = "black";
    });
  }

  if (event.key.toLowerCase() == "b") {
    console.log("Black Theme");

    themeElements.forEach((element) => {
      console.log(element);

      element.style.backgroundColor = "rgb(27, 26, 26)";
      element.style.color = "white";
    });

    document.querySelectorAll("a").forEach((a) => {
      a.style.color = "white";
    });
  }
});

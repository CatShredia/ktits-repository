// domLoaded
document.addEventListener("DOMContentLoaded", function domLoaded() {
  console.log("Dom loaded");

  const bookList = document.querySelector("#book-list");
  const forms = document.forms;

  addDeleteEvents(bookList, forms);
  addAddBookEvents(bookList, forms);
  addFindEvent(bookList, forms);
  addHideEvent(bookList, forms);

  addImagination(forms);
});

// add Delete Events
function addDeleteEvents(bookList) {
  console.log("Add delete events");

  bookList.addEventListener("click", function deleteEvent(elem) {
    //click delete button
    if (elem.target.className == "delete") {
      bookList.children[1].removeChild(elem.target.parentElement);
    }
  });
}

//add addForm Events
function addAddBookEvents(bookList, forms) {
  console.log("Add addBook events");
  const addForm = forms["add-book"];
  // clone element
  const cloneElement = bookList.children[1].children[0];

  addForm.addEventListener("submit", function addFormEvents(elem) {
    const value = addForm.querySelector('input[type="text"]').value;

    console.log("submit!" + value);

    //edit addElement
    let addElement = cloneElement.cloneNode(true);
    console.log(addElement);
    addElement.children[0].innerHTML = value;

    //add new Element do DOM
    bookList.children[1].appendChild(addElement);

    // for don't page update
    elem.preventDefault();
  });
}

//add findEvent
function addFindEvent(bookList, forms) {
  console.log("Add search event");

  const findInput = document.querySelector("#search-input");
  console.log(findInput);

  findInput.addEventListener("keyup", function findEvent(elem) {
    let formValue = findInput.value;
    let allValue = Array.from(document.querySelectorAll(".name"));

    allValue.forEach((element) => {
      console.log(typeof element.value);
      //   if (element.value.indexOf(formValue) != -1) {
      //     console.log("yes");
      //   } else {
      //     console.log("no");
      //   }
    });
  });
}

//add hideEvent
function addHideEvent(bookList) {
  console.log("Add hide event");

  const hidePoint = document.querySelector("#hide");

  console.log(hidePoint);

  hidePoint.addEventListener("change", function hudeEvent(elem) {
    if (hidePoint.checked) {
      bookList.style.display = "none";
    } else {
      bookList.style.display = "block";
    }
  });
}

// add imagination event
function addImagination(forms) {
  console.log("Add imagination event");

  const imaginationForm = forms["imagination-form"];
  const imaginationDivImage = document.querySelector(".imperial-images");
  const imaginationCheck = imaginationForm.children[1];

  const img = document.createElement("img");

  let srces = ["img1.jpg", "im2.jpg", "im3.jpg"];
  img.className = "imperial-img";

  imaginationCheck.addEventListener("change", function imaginationEvent() {
    if (imaginationCheck.checked) {
      for (let i = 0; i <= 2; i++) {
        let imgC = img.cloneNode(true);
        imgC.src = srces[i];
        imaginationDivImage.appendChild(imgC);
      }
    } else {
      imaginationDivImage.innerHTML = null;
    }
  });
}

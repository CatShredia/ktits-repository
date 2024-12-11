const form = document.forms[0]; //all forms

const isDevMode = true; //dev mode
const countPage = 4; //start count in page
const countHtml = 10; //count in html document

const products = document.querySelectorAll(".product");
const filterCheckBoxs = document.querySelectorAll(".filter-checkbox");

let filters = [];

//TODO: dom loaded
document.addEventListener("DOMContentLoaded", function () {
  let catalogClass = new CatalogClass();
});

// TODO: catalog
class CatalogClass {
  constructor() {
    //dev mode
    if (isDevMode) {
      console.log("-----------------------");

      console.log(form);
      console.log(products);

      console.log(countHtml);
      console.log(countPage);

      console.log(filterCheckBoxs);

      this.addEvents();

      this.displayProducts();

      console.log("-----------------------");
    }
  }
  // TODO: filter
  addEvents() {
    //if filter active

    form.addEventListener("change", (ivent) => {
      if (ivent.target.id == "filter-checkbox1") {
        Array.from(filterCheckBoxs).forEach((elem) => {
          elem.checked = false;
        });
        filterCheckBoxs[0].checked = true;
      } else {
        filterCheckBoxs[0].checked = false;
      }
      // change array
      filters.splice(0, filters.length); //delete all elements

      if (document.querySelector("#filter-checkbox1").checked == false) {
        Array.from(filterCheckBoxs).forEach((elem) => {
          if (elem.checked == true) {
            // console.log(elem.previousElementSibling.innerHTML);
            filters[filters.length] = elem.previousElementSibling.innerHTML;
          }
        });
      }
      this.displayProducts();
    });
  }
  //   TODO: products
  displayProducts() {
    // console.log(filters);

    const ul = document.querySelectorAll(".product-ul");

    if (filters.length == 0) {
      //раскрываем все
      Array.from(products).forEach((product) => {
        product.style.display = "grid";
      });
    } else {
      //скрываем все
      Array.from(products).forEach((product) => {
        product.style.display = "none";
      });
      //перебор ul
      for (let i = 0; i < ul.length; i++) {
        //перебор li

        var ulChildren = ul[i].children;

        for (let o = 0; o < ulChildren.length; o++) {
          //   console.log(ulChildren[o]);

          //перебор filter
          for (let p = 0; p < filters.length; p++) {
            if (
              ulChildren[o].innerHTML.toLowerCase() == filters[p].toLowerCase()
            ) {
              //   console.log(ulChildren[o].parentNode.parentNode);

              ulChildren[o].parentNode.parentNode.style.display = "grid";
            }
          }
        }
      }
    }
  }
}

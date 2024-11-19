// TODO: 1 задача
console.log("Hello World!");

let salaries = new Map();
let summ = 0;

salaries.set("John", 100).set("Ann", 160).set("Pete", 130);

console.log(salaries);

salaries.forEach((element) => {
  summ += element;
});

console.log(summ);
// TODO: 2 задача

function multiplyNumeric(obj) {
  for (let key in obj) {
    if (typeof obj[key] === "number") {
      obj[key] *= 2;
    }
  }
}

let menu = {
  width: 200,
  height: 300,
  title: "My menu",
};

multiplyNumeric(menu);

console.log(menu);
// TODO: 3 задача
let calculator = {
  a: 0,
  b: 0,

  read: function () {
    this.a = +prompt("Введите значение a:", 0); // Ввод значения a
    this.b = +prompt("Введите значение b:", 0); // Ввод значения b
  },

  sum: function () {
    return this.a + this.b;
  },

  mul: function () {
    return this.a * this.b;
  },
};

// Вызов
// calculator.read();
// alert(calculator.sum());
// alert(calculator.mul());

// TODO: 4 задача
function extractCurrencyValue(str) {
  return Number(str.substring(1));
}
console.log(extractCurrencyValue("$123330"));

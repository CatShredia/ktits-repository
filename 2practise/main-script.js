console.log("Hello, this is 2 practice from 2 diplom");

function interpolitionTest() {
  //просто для теста
  let string1 = "Hello World!";

  console.log(`Он мне сказал: ${string1}`);
}

// TODO: task1
function task1() {
  let admin;
  let name = "Джон";

  admin = name;

  alert(name);
}

// TODO: task2
//Необходимо через prompt узнать у пользователя Название Города, Год образования и Население.Вычислить возраст города Вывести сообщение: "Городу [ наименование города] исполнилось [рассчитать с момента появления] лет с момента его образования. Население - количество] человек"

function task2() {
  let cityName = prompt("Введите название города:");
  let yearFounded = parseInt(prompt("Введите год образования города:"));
  let population = parseInt(prompt("Введите население города:"));

  let currentYear = new Date().getFullYear();

  let cityAge = currentYear - yearFounded;

  alert(
    `Городу ${cityName} исполнилось ${cityAge} лет с момента его образования. Население - ${population} человек.`
  );
}

// TODO: task3
//Напишите скрипт, который находит площадь круга с радиусом r.
function task3() {
  let r = parseInt(prompt("Введите радиус: "));
  let s = Math.PI * r ** 2;

  return s;
}

// TODO: task4
//Запросите у пользователя два числа и сохраните их в переменные, выведите их сумму, разность, частное, произведение.
function task4() {
  let number1 = parseInt(prompt("Введите первое число: "));
  let number2 = parseInt(prompt("Введите второе число: "));

  console.log(`Сложение: ${number1} + ${number2} = ${number1 + number2}`);
  console.log(`Разность: ${number1} - ${number2} = ${number1 - number2}`);
  console.log(`Произведение: ${number1} * ${number2} = ${number1 * number2}`);
  console.log(`Частное: ${number1} / ${number2} = ${number1 / number2}`);
}

// TODO: start

task1();
task2();
alert(task3());
task4();

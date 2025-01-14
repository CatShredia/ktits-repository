"use strict"

let firstName = prompt("Введите фамилию");
let secondName = prompt("Введите имя");
let otchestvo = prompt("Введите отчество");

alert("Результат " + firstName + " " + secondName + " " + otchestvo);

let isTrue = confirm(`Вы уверены, что вас зовут ${firstName + secondName + otchestvo}`);

alert(isTrue)
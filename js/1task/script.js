console.log("Hello World!");

function range(firstIndex, secondIndex, step) {
  let summ = 0;
  if (step == undefined) {
    step = 1;
  }
  for (let i = firstIndex; i < secondIndex; i += step) {
    summ += i;
    console.log(i);
  }

  return summ;
}

console.log(range(1, 10, 3));
console.log("----");

function summArray() {
  let summ = 0;
  for (let i = 0; i < arguments.length; i++) {
    summ += arguments[i];
  }

  return summ;
}

console.log(summArray(1, 100, 68234, 871, 19, -713, -5623753573));
console.log("----");

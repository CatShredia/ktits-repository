'use strict'
// SM1
// alert('Hello')

// SM2
// let city = prompt('Введите название города');
// let year = +prompt('Введите год образования');
// let people = +prompt('Введите кол-во населения');

// let old = 2025 - year;

// let message = `
//     Городу ${city} исполнилось ${old} лет. 
//     Население ${people} ч.
// `;

// alert(message);

// SM3
// let money = +prompt('Сколько денег у покупателя');
// let price = +prompt('Сколько стоит товар');

// if(money == price){
//     alert('Покупка совершена без сдачи!');
// }else if(money > price){
//     let sdacha = money-price;
//     alert(`Покупка совершена, сдача ${sdacha} р.`);
// }else{
//     alert(`Покупка не совершена, сдача ${price - money} р.`);
// }

// SM4
// let box = document.querySelector('.box');

// box.style.color='red';
// box.style.backgroundColor='blue';
// box.style.padding='10px';

// let result = document.getElementById('result');

// let money = +prompt('Сколько денег у покупателя');
// let price = +prompt('Сколько стоит товар');

// if(money == price){
//     result.textContent = 'Покупка совершена без сдачи!';
// }else if(money > price){
//     let sdacha = money-price;
//     result.textContent = `Покупка совершена, сдача ${sdacha} р.`;
// }else{
//     result.textContent = `Покупка не совершена, не хватает ${price - money} р.`;
// }


// SM5
// let count = +prompt('Введите число');

// let box = '<div class="box"></div>';
// let boxList = document.querySelector('.box_list');

// // boxList.innerHTML = box;
// for(let x = 0; x < count; x++){
//     boxList.insertAdjacentHTML('beforeend',box);
// }

// SM6

let news = [
    {
        id:1,
        name:'name1',
        author:'ruslan',
        date:'12.12.2112',
        text:'lorem lorem lorem lorem lorem '
    },
    {
        id:2,
        name:'name2',
        author:'ruslanc',
        date:'13.12.2112',
        text:'lorem h gh lorem lorem lorem lorem '
    },
    {
        id:3,
        name:'name3',
        author:'ruslan3',
        date:'14.12.2112',
        text:'lorem df fd lorem lorem lorem lorem '
    },
];

let newsList = document.querySelector('.news_list');

function showNews(){
    for(let y = 0; y < news.length; y++){
        newsList.insertAdjacentHTML('beforeend',`
            <div class="news_item">
                <h3>${news[y].name}</h3>
                <p>${news[y].text}</p>
    
                <hr>
            </div>
        `)
    }
}

showNews();

let addButton = document.getElementById('addButton');

let newsName = document.getElementById('newsName');
let newsText = document.getElementById('newsText');

addButton.addEventListener('click',function(){
    let newsItem = {
        id:3,
        name:newsName.value,
        author:'ruslan3',
        date:'14.12.2112',
        text:newsText.value
    }

    news.unshift(newsItem)
    
    newsName.value='';
    newsText.value='';

    newsList.innerHTML='';
    showNews();l
})
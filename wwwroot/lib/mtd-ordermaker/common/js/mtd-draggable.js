/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.

    This file is part of MTD OrderMaker.
    MTD OrderMaker is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see  https://www.gnu.org/licenses/.
*/

let dragSrcEl = null;
let dragVaule = false;

handleDragStart = e => {    
    dragVaule = e.target.attributes.getNamedItem('aria-checked').nodeValue;
    dragSrcEl = e.target;
    e.dataTransfer.setDragImage(new Image(), 0, 0);
    e.dataTransfer.effectAllowed = 'move';
    e.dataTransfer.setData('text/html', e.target.outerHTML);
}

handleDragOver = e => {

    if (e.preventDefault) {
        e.preventDefault();
    }

    e.target.classList.add("over");
    e.dataTransfer.dropEffect = 'move';
    return false;
}

handleDragLeave = e => {
    e.target.classList.remove('over');
};

handleDrop = e => {

    if (e.stopPropagation) {
        e.stopPropagation();
    }

    if (dragSrcEl != e.target) {

        
        e.target.parentNode.removeChild(dragSrcEl);
        var dropHTML = e.dataTransfer.getData('text/html');
        e.target.insertAdjacentHTML('beforebegin', dropHTML);
        var dropElem = e.target.previousSibling;
        dropElem.className = "mtd-desk-draggable-item mdc-list-item";
        new mdc.ripple.MDCRipple(dropElem);
        addDnDHandlers(dropElem);
        if (dragVaule === "true") {
            dropElem.click();
        }

    }
    e.target.classList.remove('over');
    return false;
}

handleDragEnd = e => {
    e.target.classList.remove('over');

    let strData = "";
    const list = document.getElementById("drgList");
    const clicker = document.getElementById("drgSequence");
    const data = document.getElementById("drgData");

    list.querySelectorAll('[data-value]').forEach((item) => {
        const d = item.getAttribute("data-value");
        strData += `${d}&`;
    });

    data.value = strData;
    clicker.click();
}

addDnDHandlers = elem => {
    elem.addEventListener('dragstart', handleDragStart, false);
    elem.addEventListener('dragover', handleDragOver, false);
    elem.addEventListener('dragleave', handleDragLeave, false);
    elem.addEventListener('drop', handleDrop, false);
    elem.addEventListener('dragend', handleDragEnd, false);
}

(() => {
    const cols = document.querySelectorAll('#drgList .mtd-desk-draggable-item');
    [].forEach.call(cols, addDnDHandlers);
})();
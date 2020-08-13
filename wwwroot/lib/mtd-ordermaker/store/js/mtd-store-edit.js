﻿/*
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


//Start

const tagName = "mtdSelector";
const items = document.querySelectorAll(`div[${tagName}]`);

items.forEach((item) => {

    const id = item.attributes.getNamedItem(tagName).nodeValue;
    const input = document.getElementById(id);
    const href = document.getElementById(`${id}-href`);
    const select = document.getElementById(`${id}-select`);
    const strike = document.getElementById(`${id}-strike`);

    const actionDelete = document.getElementById(`${id}-action-delete`);
    const actionUndo = document.getElementById(`${id}-action-undo`);
    const fixDelete = document.getElementById(`${id}-delete`);

    const isFile = href.firstElementChild.textContent;
    if (isFile) actionDelete.hidden = false;

    item.addEventListener("click", () => {
        input.click();
    });

    input.addEventListener("change", (event) => {
        select.innerText = event.target.files[0].name;
        href.hidden = true; select.hidden = false; strike.hidden = true;
        actionDelete.hidden = false;
        actionUndo.hidden = true;
        fixDelete.checked = false;
    });

    actionDelete.addEventListener("click", () => {

        if (input.value) {
            href.hidden = false; select.hidden = true; strike.hidden = true;
            input.value = null;
            if (!isFile) {
                actionDelete.hidden = true;
            }
        } else {
            href.hidden = true; select.hidden = true; strike.hidden = false;
            actionDelete.hidden = true;
            actionUndo.hidden = false;
            fixDelete.checked = true;
        }
    });

    actionUndo.addEventListener("click", () => {
        href.hidden = false; select.hidden = true; strike.hidden = true;
        actionDelete.hidden = false;
        actionUndo.hidden = true;
        fixDelete.checked = false;
    });

});

document.querySelectorAll('select[datalink]').forEach((datalink) => {
    const id = datalink.attributes.getNamedItem("datalink").nodeValue;
    const input = document.getElementById(`${id}-datalink`);

    const dlv = datalink.options[datalink.selectedIndex];
    if (dlv) {
        input.value = dlv.textContent;
        datalink.addEventListener('change', (e) => {
            document.getElementById(`${id}-datalink`).value = e.target.options[e.target.selectedIndex].textContent;
        });
    }

});

const dialog = document.getElementById('dialog-info');
if (dialog) {
    const dialogInfo = new mdc.dialog.MDCDialog(dialog);
    const dialogInfoContent = document.getElementById('dialog-info-content');
    const dialogInfoTitle = document.getElementById('dialog-info-title');
    document.querySelectorAll('[mtd-info]').forEach((item) => {
        item.addEventListener('click', (e) => {
            const note = item.getAttribute('mtd-info');
            dialogInfoTitle.innerText = e.target.textContent;
            dialogInfoContent.innerText = note;
            dialogInfo.open();

        });
    });
}

const textFields = document.querySelectorAll(".mdc-text-field");

textFields.forEach((textField) => {
    new MTDTextField(textField.id)
});



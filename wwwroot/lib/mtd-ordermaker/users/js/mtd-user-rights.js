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

const partsOpenClose = (id) => {

    let display = "none";
    let iconName = "add_box";
    const icon = document.getElementById(`${id}-icon`);

    if (icon.innerText === iconName) {
        iconName = "indeterminate_check_box";
        display = "";
    };


    const rowsParts = document.querySelectorAll(`[mtd-rights-parent='${id}']`);
    rowsParts.forEach((part) => {
        part.style.display = display;
    });
    icon.innerText = iconName;
}

const selectPartsAll = (id, rightName, checked = true) => {
    const rowsParts = document.querySelectorAll(`[mtd-rights-parent='${id}']`);
    rowsParts.forEach((part) => {
        const idPart = part.getAttribute("mtd-rights-parts");
        const input = document.getElementById(`${idPart}${rightName}`);
        input.checked = checked;
    });
}

const handlerCreate = () => {
    const forms = document.querySelectorAll(`[mtd-rights]`);
    forms.forEach((form) => {
        const id = form.getAttribute("mtd-rights");
        const inputCreate = document.getElementById(`${id}-create`);
        inputCreate.addEventListener('change', () => {
            selectPartsAll(id, '-part-create', inputCreate.checked);
        });
    });
}

const handlerOpenClose = () => {
    const rowsParts = document.querySelectorAll(`[mtd-rights]`);
    rowsParts.forEach((item) => {
        item.addEventListener('click', () => {
            const id = item.getAttribute("mtd-rights");
            partsOpenClose(id);
        });
    });
}

const handlerEventChecked = () => {

    const boxes = document.querySelectorAll("[mtd-rights]");
    const rights = ["view", "edit", "delete"];

    boxes.forEach((item) => {
        const id = item.getAttribute('mtd-rights');

        rights.forEach((right) => {

            const idAll = `${id}-${right}`;
            const idOwn = `${id}-${right}-own`;          
            const idGroup = `${id}-${right}-group`;
            const inputAll = document.getElementById(idAll);
            const inputOwn = document.getElementById(idOwn);
            const inputGroup = document.getElementById(idGroup);

            inputOwn.addEventListener('change', () => {
                if (inputOwn.checked) {
                    inputAll.checked = false;
                    inputGroup.checked = false;
                }

                if (right !== "delete") {
                    selectPartsAll(id, `-part-${right}`, inputOwn.checked);
                }

            });

            inputAll.addEventListener('change', () => {
                if (inputAll.checked) {
                    inputOwn.checked = false;
                    inputGroup.checked = false;
                }
                if (right !== "delete") {
                    selectPartsAll(id, `-part-${right}`, inputAll.checked);
                }

            });

            inputGroup.addEventListener('change', () => {
                if (inputGroup.checked) {
                    inputOwn.checked = false;
                    inputAll.checked = false;
                }
                if (right !== "delete") {
                    selectPartsAll(id, `-part-${right}`, inputGroup.checked);
                }

            });
        })
    });
}

//Start

    handlerCreate();
    handlerOpenClose();
    handlerEventChecked();

    const dialog = new mdc.dialog.MDCDialog(document.getElementById('dialog-policy-delete'));
    document.getElementById('policy-open-dialog').addEventListener('click', () => {
        dialog.open();
    });
    
   

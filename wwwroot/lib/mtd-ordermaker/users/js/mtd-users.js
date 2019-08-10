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


const ClearCheckboxAll = () => {
    const rows = document.querySelectorAll(`tr[mtd-data-row]`);
    rows.forEach((row) => {

        const id = row.attributes.getNamedItem('mtd-data-row').nodeValue;
        const input = document.getElementById(`${id}-checkbox`);

        const act = document.querySelector(`[mtd-data-action="${id}"]`);
        input.checked = false;
        act.style.display = 'none';
        row.className = "";
    });
}

const ClickEventRow = (row) => {

    const id = row.attributes.getNamedItem('mtd-data-row').nodeValue;
    const input = document.getElementById(`${id}-checkbox`);

    const state = input.checked;
    ClearCheckboxAll();
    if (!state) {
        input.checked = true;
        const act = document.querySelector(`[mtd-data-action="${id}"]`);
        const css = row.attributes.getNamedItem('mtd-data-class').nodeValue;
        act.style.display = 'table-row';
        row.className = css;
    } else {
        input.checked = false;
        const act = document.querySelector(`[mtd-data-action="${id}"]`);
        act.style.display = 'none';
    }
}


(() => {

    const dialog = new mdc.dialog.MDCDialog(document.getElementById('dialog-users-delete'));
    document.querySelectorAll('a[mtd-data-row-delete]').forEach((item) => {

        item.addEventListener('click', () => {
            const id = item.getAttribute('mtd-data-row-delete');
            if (id) {
                document.getElementById('user-delete-id').value = id;
                dialog.open();
            }
        });
    });

    const rows = document.querySelectorAll(`tr[mtd-data-row]`);

    rows.forEach((row) => {
        row.addEventListener('click', (e) => {
            if (e.target.type === 'checkbox') return false;
            ClickEventRow(row);
        });
        const id = row.attributes.getNamedItem('mtd-data-row').nodeValue;
        const input = document.getElementById(`${id}-checkbox`);
        input.addEventListener('click', (e) => {
            e.target.checked = !e.target.checked;
            ClickEventRow(row);
        });

    });

})();
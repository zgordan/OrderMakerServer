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

(() => {

    const dialog = new mdc.dialog.MDCDialog(document.getElementById('dialog-users-delete'));
    document.getElementById('users-open-dialog').addEventListener('click', () => {
        dialog.open();
    });

    const dialogTransfer = new mdc.dialog.MDCDialog(document.getElementById('dialog-user-transfer'));
    document.getElementById('transfer-open-dialog').addEventListener('click', () => {
        dialogTransfer.open();
    });

    const selectPart = new mdc.select.MDCSelect(document.getElementById("users-edit-role"));

    const cpqViewAll = document.getElementById("cpq-view-all");
    const cpqViewGroup = document.getElementById("cpq-view-group");
    const cpqViewOwn = document.getElementById("cpq-view-own");

    cpqViewAll.addEventListener("change", () => {
        cpqViewGroup.checked = false;
        cpqViewOwn.checked = false;
        cpqViewAll.checked = true;
    });

    cpqViewGroup.addEventListener("change", () => {
        cpqViewAll.checked = false;
        cpqViewOwn.checked = false;
        cpqViewGroup.checked = true;
    });

    cpqViewOwn.addEventListener("change", () => {
        cpqViewAll.checked = false;
        cpqViewGroup.checked = false;
        cpqViewOwn.checked = true;
    });

})();
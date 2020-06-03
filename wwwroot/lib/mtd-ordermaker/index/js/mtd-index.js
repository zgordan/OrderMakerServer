/*
    OrderMaker - http://ordermaker.org
    Copyright(c) 2019 Oleg Bruev. All rights reserved.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.If not, see https://www.gnu.org/licenses/.
*/


const IndexShowModal = (show = true, indicator = true) => {

    const fab = document.getElementById("indexCreator");
    const modal = document.getElementById("indexModal");
    const progress = document.getElementById("indexProgress");

    modal.style.display = show ? "" : "none";
    if (fab) { fab.style.display = show ? "none" : ""; }
    progress.style.display = indicator ? "" : "none";
}

const ListenerPageMenu = () => {

    const indexPageMenu = new mdc.menu.MDCMenu(document.getElementById('indexPageMenu'));
    const pb = document.querySelector('[mtd-data-page]');
    const formId = pb.attributes.getNamedItem('mtd-data-page').nodeValue;
    pb.addEventListener('click', () => {
        indexPageMenu.open = true;
    });

    const ms = document.getElementById("indexMenuSize");
    ms.addEventListener('click', (e) => {
        const pages = e.target.getAttribute("data-value");
        document.body.scrollTop = document.documentElement.scrollTop = 0;
        IndexShowModal();
        var xmlHttp = new XMLHttpRequest();
        xmlHttp.open("POST", `/api/index/${formId}/pagesize/${pages}`, true);

        xmlHttp.send();
        xmlHttp.onreadystatechange = function () {
            if (xmlHttp.readyState == 4 && xmlHttp.status == 200) {
                //document.location.reload(location.hash, '');
                document.location.reload();
            }
        }
    });
}


(() => {

    ListenerPageMenu();

    const storeIds = document.getElementById("nav-store-ids");
    if (storeIds) {
        sessionStorage.setItem("storeIds", storeIds.value);
    }

    new MTDTextField("search-text");
    new MTDTextField("search-number");

})();
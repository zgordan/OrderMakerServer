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

    document.querySelectorAll("div[mtd-file-upload]").forEach((block) => {
        const id = block.getAttribute("mtd-file-upload");
        const result = document.getElementById(`${id}-file-upload-result`);
        const resultText = result.getAttribute("mtd-file-upload-text");
        const input = document.getElementById(`${id}-file-upload-input`);                
        const cancel = document.getElementById(`${id}-file-upload-cancel`);

        input.addEventListener("change", () => {
            var filename = input.value;
            if (/^\s*$/.test(filename)) {
                block.classList.remove('active');
                result.innerText = resultText;
            }
            else {
                block.classList.add('active');
                result.innerText = filename.replace("C:\\fakepath\\", "");
            }
        });

        cancel.addEventListener('click', () => {
            block.classList.remove('active');
            result.innerText = resultText;
            input.value = "";
            cancel.blur();
        });
        
    });

})();
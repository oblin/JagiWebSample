(function () {
    'use strict';

    window.app.controller('codeController', codeController);
    codeController.$inject = ['model', 'alerts'];

    function codeController(model, alerts) {
        var vm = this;
        vm.list = model;
        // control paging
        vm.pagedList = { list: [], count: 0, currentPage: 1, pageCount: 10 };
        vm.hadBeenModified = hadBeenModified;
        vm.nextPage = nextPage;
        vm.prevPage = prevPage;
        vm.goPage = goPage;

        vm.search;
        vm.searching = searching;

        // control list selection
        vm.current = model[0];
        vm.selected = selected;
        vm.select = select;
        vm.updateCurrentToList = updateCurrentToList;
        vm.removeCurrentFromList = removeCurrentFromList;

        setPagedList(vm.list, 1);

        /**
         * 將傳入的 list 轉成 page list
         * @param array list 
         */
        function setPagedList(list, page) {
            var pagedList = vm.pagedList;
            var pageCount = pagedList.pageCount;
            page = page || ((pagedList.currentPage - 1) * pageCount > list.length ? 1 : pagedList.currentPage);
            vm.pagedList.count = Math.ceil(list.length / pageCount);
            vm.pagedList.currentPage = page;

            vm.pagedList.list = list.slice((page - 1) * pageCount, page * pageCount);
        }

        /**
         * 檢查 Form inputs 是否有被變更，如果有，則不允許點選上、下頁等功能
         *  vm.current.isDirty 是由 child-scope 設定（codeEditDirective.js 中，觀察 form.$dirty 變化）
         */
        function hadBeenModified() {
            if (vm.current.isDirty)
                return vm.current.isDirty;
            return false;
        }

        /**
         * 設定下一頁，考量可能有搜尋，因此必須要搭配使用
         */
        function nextPage() {
            if (vm.pagedList.currentPage == vm.pagedList.count)
                return;
            vm.pagedList.currentPage++;
            searching();
        }

        function prevPage() {
            if (vm.pagedList.currentPage <= 1) {
                return;
            }
            vm.pagedList.currentPage--;
            searching();
        }

        function goPage() {
            if (vm.pagedList.currentPage > vm.pagedList.count) {
                vm.pagedList.currentPage = vm.pagedList.count;
            }
            if (vm.pagedList.currentPage < 1) {
                vm.pagedList.currentPage = 1;
            }
            searching();
        }

        /**
         * 搜尋 model list，並僅列出符合條件的結果
         */
        function searching() {
            var searchWord = "";
            if (vm.search)
                searchWord = vm.search.toUpperCase();

            if (searchWord.length > 0) {
                var searchList = [];
                for (var i = 0; i < model.length; i++) {
                    var item = model[i];
                    if (item.itemType.length > 0 ? item.itemType.toUpperCase().indexOf(searchWord) > -1 : false
                        || item.typeName.length > 0 ? item.typeName.toUpperCase().indexOf(searchWord) > -1 : false
                        || item.parentCode.length > 0 ? item.parentCode.toUpperCase().indexOf(searchWord) > -1 : false)
                        searchList.push(item);
                }
                setPagedList(searchList);
            } else {
                setPagedList(model);
            }
        }

        /**
         * 判斷 table list 中的項目是否需要標註 active
         * @param item list 中的 items
         */
        function selected(item) {
            if (vm.current)
                return vm.current.id == item.id ? 'active' : null;
            return null;
        }

        function select(item) {
            if (hadBeenModified()) {
                alerts.warning("尚未存檔，必須選擇存檔或者取消後，才能進行下一項作業！");
                return;
            }

            vm.current = item;
        }

        function updateCurrentToList(item) {
            vm.current = item;
            vm.list.unshift(item);
            setPagedList(vm.list, 1);
        }

        function removeCurrentFromList(item) {
            var idx = vm.list.getIndexById(item.id);
            vm.list.splice(idx, 1);
            vm.current = vm.list[idx];
            setPagedList(vm.list);
        }
    }
})();
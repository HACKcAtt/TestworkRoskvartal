// Переменная для отслеживания скорости ввода (используется в работе с поисковой строкой.
var typeTimer = null;

// Write your JavaScript code.
$(document).ready(function () {
    // Применение HTML5 History API для удобной навигации кнопками "вперёд" и "назад", pushState - внесение изменения в браузерную строку.
    $(window).on('popstate', function (e) {
        if (e.originalEvent.state != null) {
            showInstancesTable({ url: e.originalEvent.state.url, ajax: e.originalEvent.state.ajax, pushState: false });
        } else {
            location.reload();
        }
    });

    createInstanceNameDuplicationInDatabaseChecking();
    createInstanceEmailDuplicationInDatabaseChecking();
    createInstanceDateTimeForAppointmentChecking();
    showBlockListener();
    hideBlockListener();

    // Обработка нажатия ссылки создания нового профиля.
    $("#create_instance").on("click", function (event) {
        // Предотвращение выполнения стандартного действия (перехода по ссылке).
        event.preventDefault(event);
        // Непосредственное выполнение функции обработчика - отрисовка модального окна с добавлением нового профиля.
        // Получение адреса ссылки (по атрибуту "href").
        var url = $(this).attr("href");
        showCreateModalInstance(url);
        // Скрытие лишнего scroll-элемента, появляющегося из-за создания доступности прокрутки модального окна правки, появляющегося из окна деталей.
        if ($('#modal_create_instance').is(':visible')) {
            document.body.style.overflow = "hidden";
        }
        // "return false" необходимо для браузеров Firefox, без этого работает в них некорректно.
        return false;
    });

    // Ожидание нажатия ссылки сортировки.
    setListenerSortLink();

    // Обработка пагинации.
    $("body").on("click", ".pagination a", function (event) {
        // Предотвращение выполнения стандартного действия.
        event.preventDefault(event);
        // Непосредственное выполнение функции обработчика.
        var $button = $(this);
        if (document.getElementsByClassName('page') != null) {
            showInstancesTable({ url: $button.attr("href"), ajax: $button.data("pagination-link") });
        }
        // "return false" необходимо для браузеров Firefox, без этого работает в них некорректно.
        return false;
    });

    // В случае выбора иного количества отделений для отрсивоки на одной странице отрсиовать таблицу отделений в соответствии с этим числом.
    $(".instances-per-page-for-display").change(function () {
        showInstancesTable();
    });

    // Обработка правки.
    $("body").on("click", ".instances-edit", function (event) {
        // Предотвращение выполнения стандартного действия.
        event.preventDefault(event);
        // Непосредственное выполнение функции обработчика.
        var $button = $(this);
        // Применение HTML5 History API для удобной навигации кнопками "вперёд" и "назад", pushState - внесение изменения в браузерную строку. Отрисовка в нужном виде.
        showCreateModalInstance($button.attr('href'));
        // "return false" необходимо для браузеров Firefox, без этого работает в них некорректно.
        return false;
    });

    // Обработка деталей.
    $("body").on("click", ".instance-details", function (event) {
        // Предотвращение выполнения стандартного действия.
        event.preventDefault(event);
        // Непосредственное выполнение функции обработчика.
        var $button = $(this);
        // Применение HTML5 History API для удобной навигации кнопками "вперёд" и "назад", pushState - внесение изменения в браузерную строку. Отрисовка в нужном виде.
        showInstanceDetailsModal($button);
        // "return false" необходимо для браузеров Firefox, без этого работает в них некорректно.
        return false;
    });

    // Обработка удаления.
    $("body").on("click", ".instance-delete", function (event) {
        // Предотвращение выполнения стандартного действия.
        event.preventDefault(event);
        // Непосредственное выполнение функции обработчика.
        var $button = $(this);
        // Применение HTML5 History API для удобной навигации кнопками "вперёд" и "назад", pushState - внесение изменения в браузерную строку. Отрисовка в нужном виде.
        showInstanceDetailsModal($button);
        // "return false" необходимо для браузеров Firefox, без этого работает в них некорректно.
        return false;
    });

    // Отрисовка окна правки, появляющегося из окна деталей.
    $("body").on("click", ".instances-edit-from-details", function (event) {
        // Непосредственное выполнение функции обработчика.
        var $button = $(this);
        // Предотвращение выполнения стандартного действия.
        event.preventDefault(event);
        $("#modal_details_instance").modal("hide");
        // Обработка деталей.
        $("#modal_details_instance").on("hidden.bs.modal", function (event) {
            // Предотвращение выполнения стандартного действия.
            event.preventDefault();
            showCreateModalInstance($button.attr('href'));
            // "return false" необходимо для браузеров Firefox, без этого работает в них некорректно.
            return false;
        });
        // Создание доступности прокрутки модального окна правки, появляющегося из окна деталей.
        $("#modal_create_instance").css("overflow", "auto");
        // Скрытие лишнего scroll-элемента, появляющегося из-за создания доступности прокрутки модального окна правки, появляющегося из окна деталей.
        if ($('#modal_create_instance').is(':visible')) {
            document.body.style.overflow = "hidden";
        }
        // "return false" необходимо для браузеров Firefox, без этого работает в них некорректно.
        return false;
    });

    // Обработка поиска.
    $("body").on("click", "#instance_search_button", function (event) {
        // Предотвращение выполнения стандартного действия.
        event.preventDefault(event);
        // Непосредственное выполнение функции обработчика.
        var $button = $(this);
        console.log('11');
        // Получаем поисковую строку.
        var searchString = $("#instance_search_input_field").val();
        console.log('12');
        console.log($("#instance_search_input_field").val());
        if (searchString == "") {
            showEmptySearchStringAlert();
        }
        // Отрисовка в нужном виде.
        showInstancesTable({ url: $button.data("href"), ajax: $button.data("ajax"), searchString: searchString });
        console.log('13');
        if (($(".instance-table-item").find($("tr")).length) <= 1) {
            console.log('14');
            showUnsuccessfullSearchingAlert();
        }
        // "return false" необходимо для браузеров Firefox, без этого работает в них некорректно.
        return false;
    });

    // Обработка поиска сразу при вводе символов в поисковую строку.
    $("#instance_search_input_field").on('keyup', function () {
        // Интервал между нажатиями клавиш (например, 300 - это 0,3 секунды).
        var keysUpDownTimeInterval = 300;
        // При необходимости сброс таймера контроля ввода.
        if (typeTimer != null) {
            clearTimeout(typeTimer);
        }
        // Установка таймера контроля ввода на keysUpDownTimeInterval милисекунд между нажатиями клавиш. Если вводить текст поиска медленнее, то динамически начнут отрисовываться результаты поиска.
        typeTimer = setTimeout(function () {
            // Получение поисковой строки.
            var searchString = $("#instance_search_input_field").val();
            // Определение кнопки.
            var $button = $("#instance_search_button");
            // Отрисовка в нужном виде.
            showInstancesTable({ url: $button.data("href"), ajax: $button.data("ajax"), searchString: searchString });
        }, keysUpDownTimeInterval);
    });

    // Если существует форма создания профиля, то привязка кнопки создания к ней.
    if ($('#instance_form').length) {
        bindInstanceFormSubmit();
    }

    // Если существует форма создания профиля, то привязка кнопки создания к ней.
    if ($('#details_delete_instance_form').length) {
        bindFormDeleteInstance();
    }

    // Возврат scroll-элемента в страницу после закрытия окна правки/создания профиля кнопкой.
    $("body").on("click", "#buttonBackToIndexFromCreateOrEdit", function (event) {
        // Предотвращение выполнения стандартного действия.
        event.preventDefault(event);
        // Непосредственное выполнение функции обработчика.
        // Вернуть доступность scroll-элемента, так как scroll-элемент окна правки исчезает вместе с окном.
        document.body.style.overflow = "scroll";
        // "return false" необходимо для браузеров Firefox, без этого работает в них некорректно.
        return false;
    });
});

// Функция отрисовки таблицы с данными.
function showInstancesTable(ourData) {
    var $instancesList = $('#instances_list');
    var url;
    var params = {};
    // Получение количества отделений для отрисовки на одной странице.
    var instancesPerPage = $(".instances-per-page-for-display").val();
    if ($(".instances-per-page-for-display").val()) {
        if ($(".instances-per-page-for-display").val() == "All") {
            instancesPerPage = -1;
        }
        else {
            instancesPerPage = $(".instances-per-page-for-display").val();
        }
    }
    // Получение адреса ссылки.
    // Если ссылка не определена, то передать в неё текущее состояние из AJAX, и если pushState изменился из AJAX, то менять заголовок и бразуерную строку, иначе оставить изначальную ссылку.
    if (typeof ourData !== "undefined") {
        url = ourData.ajax;
        var pushState = ((typeof ourData.pushState == "undefined") || ourData.pushState);
        //if (pushState) {
        //    history.pushState({ ajax: ourData.ajax, url: ourData.url }, document.title, ourData.url);
        //}
        if (typeof ourData.searchString !== "undefined") {
            params["searchString"] = ourData.searchString;
        }
        // Для передачи количества профилей для отрисовки на одной странице.
        params["instancesPerPage"] = instancesPerPage;
    } else {
        url = $instancesList.data("url");
        // Для передачи количества профилей для отрисовки на одной странице.
        params["instancesPerPage"] = instancesPerPage;
    }
    // Функция jQuery для получения и дальнейшей отрисовки секции таблицы с данными покупателей.
    $.ajax({
        // Передача ссылки для получения html-кода.
        url: url,
        data: params,
        // Отрисовка лоад-кружка в случае долгой загрузки.
        beforeSend: function () {
            $('.loader').show();
        },
        // Функция в случае успеха, принимающая полученный html-код.
        success: function (data) {
            // Доавление в блока <div> с id customers_list в виде полученного html-кода.
            $instancesList.html(data);
            setListenerSortLink();
        },
        // В случае ошибки вернуть в консоль xml-html-request.
        error: function (xhr) {
            console.log(xhr);
        },
        //Скрытие лоад-кружка в случае окончания загрузки.
        complete: function () {
            $('.loader').hide();
        }
    });
}

// Функция обработчика - отрисовка модального окна с добавлением нового профиля.
function showCreateModalInstance(url) {

    // Функция jQuery для получения формы модального окна.
    $.ajax({
        // Передача ссылки для получения html-кода.
        url: url,
        // В data будет содержаться html-код.
        data: {},
        // Функция, в случае успеха принимающая полученный html-код.
        success: function (data) {
            // В блок <div> с id #modal_create_content в _Layout.cshtml вставка полученного html-кода, это и будет модальное окно.
            $("#modal_create_instance_content").html(data);
            bindInstanceFormSubmit();
            // Непосредственная отрисовка модального окна.
            $('#modal_create_instance').modal('show');
            if ($(".modal-dialog").length) {
                $("#linkBackToIndexFromCreateOrEdit").hide();
                $("#buttonBackToIndexFromCreateOrEdit").show();
            }
        },
        // В случае ошибки вернуть в консоль xml-html-request.
        error: function (xhr) {
            console.log(xhr);
        }
    });
}

// Функция обработчика - отрисовка модального окна с деталями профиля.
function showInstanceDetailsModal($button) {
    // Получение адреса ссылки (по атрибуту "href").
    var url = $button.attr("href");
    //bindInstanceFormSubmit();
    // Функция jQuery для получения формы модального окна.
    $.ajax({
        // Передача ссылки для получения html-кода.
        url: url,
        // В data будет содержаться html-код.
        data: {},
        // Функция в случае успеха, принимающая полученный html-код.
        success: function (data) {
            // В блок <div> с id #modal_details_content в _Layout.cshtml вставка полученного html-кода, это и будет модальное окно.
            $("#modal_details_content_instance").html(data);

            bindFormDeleteInstance();

            // Непосредственная отрисовка модального окна.
            $('#modal_details_instance').modal('show');
            if ($(".modal-dialog").length) {
                $("#button_back_to_index_from_details_or_delete").show();
                $("#link_back_to_index_from_details_or_delete").hide();
            }

            showBlockListener();
            hideBlockListener();
        },
        // В случае ошибки вернуть в консоль xml-html-request.
        error: function (xhr) {
            console.log(xhr);
        }
    });
}

// Функция привязки кнопки удаления профиля.
function bindFormDeleteInstance() {
    $(".instance-delete-it").on("click", function (event) {
        event.preventDefault(event);
        var $btn = $(this);
        var locationUrl = $("#link_back_to_index_from_details_or_delete").prop("href");
        var url = $btn.prop("href");
        $.ajax({
            // Тип запроса.
            type: "POST",
            // Ссылка для отправки.
            url: url,
            // В случае успеха вывод сериализованных данных в консоль и отрисовка формы.
            success: function () {
                // Закрыть модальное окно.
                $('#modal_details_instance').modal('hide');
                // В случае, если удаление было открыто на отдельную страницу, то сделать перенаправление на страницу со списком профилей.
                if ($('.modal-open').length == 0) {
                    // Задержка перед переодом на страницу со списком профилей со страницы удаления профиля.
                    var delayTimeBeforeRedirectionToInstancesFromDeletingPage = 2000;
                    setTimeout(function () {
                        location.href = locationUrl;
                    }, delayTimeBeforeRedirectionToInstancesFromDeletingPage);
                }
                // Отрисовать таблицу с профилями покупателей.
                showInstancesTable();
                // Сообщение в случае удаления профиля.
                showProfileSuccessfulyDeletedAlert();
            },
            // В случае неуспеха вернуть в консоль xml-html-request.
            fail: function (xhr) {
                console.log(xhr);
            }
        });
        return false;
    });
}

// Функция привязки кнопки создания нового профиля.
function bindInstanceFormSubmit() {
    // Для сериализации далее указание, что использоваться будет именно текущая форма.
    var $form = $('#instance_form');
    console.log($form.data('list-url'));
    // Форма готова.
    // Функция обработчика - отправка данных на сервер.
    $form.on("submit", function (event) {
        event.preventDefault(event);
        // Задание ссылки, по оной данные уходить будут. В данном случае применится ссылка, заданная в форме на случай действия (action).
        var url = $form.prop('action');
        // Получение необходимых данных со страницы в объект customer.
        var instance = $form.serialize();

        // Инициализация правил для стандартного валидатора.
        $form.data("validator", null);
        console.log('before_valid');
        $.validator.unobtrusive.parse(document);
        console.log('after_valid');
        // Проверка полного отсутстствия хотя бы одной секции и наличия пустых требуемых полей, $form.valid() должен быть в начале проверки, чтобы сработали проверки полей.
        if ($form.valid()) {
            $.ajax({
                // Тип запроса.
                type: "POST",
                // Ссылка для отправки.
                url: url,
                // Данные для отправки.
                data: instance,
                // В случае успеха отрисовка формы.
                success: function (data) {
                    console.log(data);
                    if (data == "") {
                        if ($('#instance_form').prop('action') == location.href) {
                            location.href = $form.data('list-url');
                            console.log($form.data('list-url'));
                        }
                        else {
                            // Закрыть модальное окно.
                            $('#modal_create_instance').modal('hide');
                            // Вернуть доступность scroll-элемента, так как scroll-элемент окна правки исчезает вместе с окном.
                            document.body.style.overflow = "scroll";
                            // Отрисовать таблицу с профилями покупателей.
                            showInstancesTable();
                            // Динамически обновить страницу "Детали", если детали профиля выводились на отдельную страницу, и была произведена правка.
                            url_d = location.href;
                            if ($('#details_delete_instance_form').length) {
                                $.ajax({
                                    url: url_d,
                                    success: function (html) {
                                        $("#details_delete_instance_form").html(html);
                                    }
                                });
                            }
                            // Сообщение в случае создания нового профиля.
                            if ($('#instance_for_addition').length) {
                                showProfileSuccessfulyAddedAlert();
                            }
                            // Сообщение в случае успешной правки существующего профиля.
                            if ($('#instance_for_edition').length) {
                                showProfileSuccessfulyEditedAlert();
                            }
                        }
                    } else {
                        $("#modal_create_instance_content").html(data);
                        bindInstanceFormSubmit();
                    }
                },
                // В случае неуспеха вернуть в консоль xml-html-request.
                fail: function (xhr) {
                    console.log(xhr);
                }
            });
        }
        // В случае полного отсутстствия хотя бы одной секции вывод предупредительного сообщения.
        else {
            showNotEnoughDataToSendAlert();
            return false;
        }
        return false;
    });
}

// Функция, выводящая на alertLifetime секунд уведомление о невозможности создания недостаточно заполненного профиля.
function showNotEnoughDataToSendAlert() {
    // Время отображения предупреждающего сообщения в милисекундах (5000 ms == 5 s) до его исчезновения.
    var alertLifetime = 5000;
    $("#danger_alert").fadeTo(alertLifetime, 500).slideUp(500, function () {
        $("#danger_alert").slideUp(500);
    });
}

// Функция, выводящая на alertLifetime секунд уведомление об успешном создании профиля.
function showProfileSuccessfulyAddedAlert() {
    // Время отображения предупреждающего сообщения в милисекундах (5000 ms == 5 s) до его исчезновения.
    var alertLifetime = 5000;
    $("#successfull_addition").fadeTo(alertLifetime, 500).slideUp(500, function () {
        $("#successfull_addition").slideUp(500);
    });
}
// Функция, выводящая на alertLifetime секунд уведомление об успешном редактировании профиля.
function showProfileSuccessfulyEditedAlert() {
    // Время отображения предупреждающего сообщения в милисекундах (5000 ms == 5 s) до его исчезновения.
    var alertLifetime = 5000;
    $("#successfull_edition").fadeTo(alertLifetime, 500).slideUp(500, function () {
        $("#successfull_edition").slideUp(500);
    });
}
// Функция, выводящая на alertLifetime секунд уведомление об успешном удалении профиля.
function showProfileSuccessfulyDeletedAlert() {
    // Время отображения предупреждающего сообщения в милисекундах (5000 ms == 5 s) до его исчезновения.
    var alertLifetime = 5000;
    $("#successfull_deleting").fadeTo(alertLifetime, 500).slideUp(500, function () {
        $("#successfull_deleting").slideUp(500);
    });
}
// Функция, выводящая на alertLifetime секунд уведомление о безуспешности проведённого поиска.
function showUnsuccessfullSearchingAlert() {
    // Время отображения предупреждающего сообщения в милисекундах (5000 ms == 5 s) до его исчезновения.
    var alertLifetime = 3000;
    $("#unsuccessfull_searching").fadeTo(alertLifetime, 500).slideUp(500, function () {
        $("#unsuccessfull_searching").slideUp(500);
    });
}

// Обработка сортировки по имени.
function setListenerSortLink() {
    $("#name_th, #name_th_2").on("click", function (event) {
        // Предотвращение выполнения стандартного действия (перехода по ссылке).
        event.preventDefault(event);
        showInstancesTable({ url: $(this).find("p").data("href"), ajax: $(this).find("p").data("href") })
        return false;
    });
}

//Обработка показа блока.
function showBlockListener() {
    $("#show_block").on("click", function (event) {
        event.preventDefault(event);
        $("#show_hide_block").show();
        $("#hide_block").show();
        $("#show_block").hide();
        return false;
    });
}

//Обработка скрытия блока.
function hideBlockListener() {
    $("#hide_block").on("click", function (event) {
        event.preventDefault(event);
        $("#show_hide_block").hide();
        $("#show_block").show();
        $("#hide_block").hide();
        return false;
    });
}

// Функция, выводящая на alertLifetime секунд уведомление о пустой поисковой строке.
function showEmptySearchStringAlert() {
    // Время отображения предупреждающего сообщения в милисекундах (5000 ms == 5 s) до его исчезновения.
    var alertLifetime = 3000;
    $("#empty_searchstring").fadeTo(alertLifetime, 500).slideUp(500, function () {
        $("#empty_searchstring").slideUp(500);
    });
}

// Вспомогательная функция проверки массива на уникальность его элементов.
function testUnique(A) {
    var n = A.length;
    for (var i = 0; i < n - 1; i++) {
        for (var j = i + 1; j < n; j++) { if (A[i] === A[j]) return false; }
    }
    return true;
}

// Функция проверки достпуности названия сущности для использования при вызове создания нового профиля.
function createInstanceNameDuplicationInDatabaseChecking() {
    var instanceNameKekFlag = true;
    // Функция проверки достпуности телефонного номера для использования.
    $("body").on('keyup', ".instance-name-input", function () {
        var $this = $(this);
        var commonInstanceName = $this.val();
        // Получение id всплывающих сообщений о доступности и недоступности.
        var alertAvaliableId = "#avaliable_instance_name_check";
        var alertUnavaliableId = "#unavaliable_instance_name_check";
        // На случай, если всплывающих сообщений нет (то есть проверка на уникальность не нужна).
        if ($(alertAvaliableId).length == 0 || $(alertUnavaliableId).length == 0)
        {
            return false; 
        }
        // Адрес для POST-запроса.
        var urlCheck = "/" + $("#controller_name").text() + "/CheckInstanceName";
        // Интервал между нажатиями клавиш (например, 1000 - это 1 секунда).
        var waitForStartChecking = 700;
        // При необходимости сброс таймера контроля ввода.
        if (typeTimer != null) {
            clearTimeout(typeTimer);
        }
        // Установка таймера контроля ввода на keysUpDownTimeInterval милисекунд между нажатиями клавиш. Если вводить текст поиска медленнее, то динамически начнут отрисовываться результаты поиска.
        typeTimer = setTimeout(function () {
            // Получение вводимого номера телефона.
            var instanceNameForCheck = $this.val();
            // POST-запрос на сервер.
            $.ajax({
                type: "POST",
                url: urlCheck,
                data: { instanceNameForCheck: instanceNameForCheck },
                // Действия по результатам ответа с сервера: если пришло 0 и в строке ввода что-то есть, то показать сообщение о доступности, иначе - о недоступности.
                success: function (response) {
                    if (response == 0) {
                        if ($this.val() != "") {
                            $(alertUnavaliableId).hide();
                            $(alertAvaliableId).show();
                            console.log('1');
                            document.getElementById('button_create').disabled = false;
                        }
                        else {
                            $(alertUnavaliableId).hide();
                            $(alertAvaliableId).hide();
                            console.log('2');
                            document.getElementById('button_create').disabled = true;
                        }
                    }
                    else {
                        if ($this.val() != "") {
                            $(alertUnavaliableId).show();
                            $(alertAvaliableId).hide();
                            console.log('3');
                            document.getElementById('button_create').disabled = true;
                            instanceNameKekFlag = false;
                        }
                        else {
                            $(alertUnavaliableId).hide();
                            $(alertAvaliableId).hide();
                            console.log('4');
                            document.getElementById('button_create').disabled = true;
                        }
                    }
                },
                // В случае неуспеха вернуть в консоль xml-html-request.
                fail: function (xhr) {
                    console.log(xhr);
                }
            });
        }, waitForStartChecking);
    });
    return instanceNameKekFlag;
}

// Функция проверки достпуности названия сущности для использования при вызове создания нового профиля.
function createInstanceEmailDuplicationInDatabaseChecking() {
    var instanceEmailKekFlag = true;
    // Функция проверки достпуности телефонного номера для использования.
    $("body").on('keyup', ".instance-email-input", function () {
        var $this = $(this);
        var commonInstanceName = $this.val();
        // Получение id всплывающих сообщений о доступности и недоступности.
        var alertAvaliableId = "#avaliable_instance_email_check";
        var alertUnavaliableId = "#unavaliable_instance_email_check";
        // На случай, если всплывающих сообщений нет (то есть проверка на уникальность не нужна).
        if ($(alertAvaliableId).length == 0 || $(alertUnavaliableId).length == 0) {
            return false;
        }
        // Адрес для POST-запроса.
        var urlCheck = "/" + $("#controller_name").text() + "/CheckInstanceEmail";
        // Интервал между нажатиями клавиш (например, 1000 - это 1 секунда).
        var waitForStartChecking = 700;
        // При необходимости сброс таймера контроля ввода.
        if (typeTimer != null) {
            clearTimeout(typeTimer);
        }
        // Установка таймера контроля ввода на keysUpDownTimeInterval милисекунд между нажатиями клавиш. Если вводить текст поиска медленнее, то динамически начнут отрисовываться результаты поиска.
        typeTimer = setTimeout(function () {
            // Получение вводимого номера телефона.
            var instanceEmailForCheck = $this.val();
            // POST-запрос на сервер.
            $.ajax({
                type: "POST",
                url: urlCheck,
                data: { instanceEmailForCheck: instanceEmailForCheck },
                // Действия по результатам ответа с сервера: если пришло 0 и в строке ввода что-то есть, то показать сообщение о доступности, иначе - о недоступности.
                success: function (response) {
                    if (response == 0) {
                        if ($this.val() != "") {
                            $(alertUnavaliableId).hide();
                            $(alertAvaliableId).show();
                            console.log('1');
                            document.getElementById('button_create').disabled = false;
                        }
                        else {
                            $(alertUnavaliableId).hide();
                            $(alertAvaliableId).hide();
                            console.log('2');
                            document.getElementById('button_create').disabled = true;
                        }
                    }
                    else {
                        if ($this.val() != "") {
                            $(alertUnavaliableId).show();
                            $(alertAvaliableId).hide();
                            console.log('3');
                            document.getElementById('button_create').disabled = true;
                            instanceEmailKekFlag = false;
                        }
                        else {
                            $(alertUnavaliableId).hide();
                            $(alertAvaliableId).hide();
                            console.log('4');
                            document.getElementById('button_create').disabled = true;
                        }
                    }
                },
                // В случае неуспеха вернуть в консоль xml-html-request.
                fail: function (xhr) {
                    console.log(xhr);
                }
            });
        }, waitForStartChecking);
    });
    return instanceEmailKekFlag;
}

// Функция проверки достпуности даты и вывода списка промежутков времени в случае, если они есть в данной дате.
function createInstanceDateTimeForAppointmentChecking() {
    var instanceDateKekFlag = true;
    var request = function () {
        var $this = $(this);
        var commonInstanceName = $this.val();
        console.log(commonInstanceName);
        if (!/\d{4}\-\d{2}-\d{2}/.test(commonInstanceName)) {
            $(alertUnavaliableId).hide();
            $(alertAvaliableId).hide();
            document.getElementById('button_create').disabled = true;
            return;
        }
        // Получение id всплывающих сообщений о доступности и недоступности.
        var alertAvaliableId = "#avaliable_appointment_date";
        var alertUnavaliableId = "#unavaliable_appointment_date";
        var doctorId = $("#doctor_id").val();
        console.log(doctorId);
        // На случай, если всплывающих сообщений нет (то есть проверка на уникальность не нужна).
        if ($(alertAvaliableId).length == 0 || $(alertUnavaliableId).length == 0) {
            return false;
        }
        // Адрес для POST-запроса.
        var urlCheck = "/" + $("#controller_name").text() + "/CheckInstanceAppointmentDate";
        // Интервал между нажатиями клавиш (например, 1000 - это 1 секунда).
        var waitForStartChecking = 1000;
        // При необходимости сброс таймера контроля ввода.
        if (typeTimer != null) {
            clearTimeout(typeTimer);
        }
        // Установка таймера контроля ввода на keysUpDownTimeInterval милисекунд между нажатиями клавиш. Если вводить текст поиска медленнее, то динамически начнут отрисовываться результаты поиска.
        typeTimer = setTimeout(function () {
            var commonInstanceName = $this.val();
            // POST-запрос на сервер.
            $.ajax({
                type: "POST",
                url: urlCheck,
                data: { commonInstanceName: commonInstanceName, id: doctorId },
                // Действия по результатам ответа с сервера: если пришло 0 и в строке ввода что-то есть, то показать сообщение о доступности, иначе - о недоступности.
                success: function (response) {
                    if (response == 0) {
                        $(alertUnavaliableId).hide();
                        $(alertAvaliableId).show();
                        var url = $('#href_id').data('href');
                        url = url + '?date=' + commonInstanceName;
                        document.getElementById('button_create').disabled = false;
                        showCreateModalInstance(url);
                    }
                    else {
                        $(alertUnavaliableId).show();
                        $(alertAvaliableId).hide();
                        document.getElementById('button_create').disabled = true;
                        instanceDateKekFlag = false;
                    }
                },
                // В случае неуспеха вернуть в консоль xml-html-request.
                fail: function (xhr) {
                    console.log(xhr);
                }
            });
        }, waitForStartChecking);
    }
    // Функция проверки достпуности для использования.
    $("body").on('keyup', ".instance-date-input", request);
    $("body").on('change', ".instance-date-input", request);
    return instanceDateKekFlag;
}
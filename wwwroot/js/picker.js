$('#datetimepicker').datetimepicker({
    format: 'YYYY-MM-DD HH:mm',
    icons: {
        time: 'far fa-clock',
        date: 'far fa-calendar',
        up: 'fas fa-arrow-up',
        down: 'fas fa-arrow-down',
        previous: 'fas fa-chevron-left',
        next: 'fas fa-chevron-right',
        today: 'fas fa-calendar-check',
        clear: 'far fa-trash-alt',
        close: 'far fa-times-circle'
    }
}).on('change.datetimepicker', function (e) {
    dotnetHelper.invokeMethodAsync('DateTimePickerChanged', e.date.format('YYYY-MM-DD HH:mm'));
});
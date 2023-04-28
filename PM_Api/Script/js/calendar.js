(function (a) {
    var d = a(window),
        c = a(document),
        b = a("body");
    a(function () {
        var g = a(".calendar--events"),
            h = g.children(".fc-events"),
            f = h.children(".fc-event"),
            i = g.find(".form-check-input");
        console.log(g.length);
        if (g.length) {
            f.each(function () {
                var j = a(this),
                    k = j.css("background-color");
                j.draggable({
                    revert: true,
                    revertDuration: 0,
                    zIndex: 999
                }).css({
                    "border-color": k
                }).data("event",
                    {
                        title: j.text(),
                        color: k,
                        stick: true
                    });
            });
        }
        g.on("click", ".calendar--event__colors li", function () {
            console.log("32313");
            var j = a(this);
            j.addClass("active").siblings().removeClass("active");
        });
        g.on("submit", "form", function (o) {
            o.preventDefault();
            console.log("创建事件");
            var l = a(this),
                n = l.children("input"),
                m = a("<div></div>"),
                k = g.find(".calendar--event__colors .active"),
                j = k.css("background-color");
            m.draggable({
                revert: true,
                revertDuration: 0,
                zIndex: 999
            }).css({
                "border-color": j
            }).data("event",
                {
                    title: n.val(),
                    color: j,
                    stick: true
                }).addClass(" fc-event " + k.attr("class")).text(n.val()).appendTo(h)
        });
        var e = a("#calendarApp");
        if (e.length) {
            console.log("加载数据");
            e.fullCalendar({
                header: {
                    left: "",
                    center: "prev next title",
                    right: "today basicDay basicWeek month"
                },
                locale: "zh-cn",
                editable: true,
                droppable: true,
                drop: function () {
                    if (i.is(":checked")) {
                        console.log("拖拽后移除");
                        a(this).remove();
                    }
                },
                timeFormat: "h(:mm)a",
                events: [
                    {
                        title: "约会",
                        start: "2021-04-01 10:30:00",
                        id: "demo1"
                    }, {
                        title: "会议",
                        start: "2021-04-10 10:30:00",
                        color: "#009378",
                        id: "demo2"
                    }, {
                        title: "午饭",
                        start: "2021-04-12 13:30:00",
                        color: "#2bb3c0",
                        id: "demo3"
                    }, {
                        title: "长会议",
                        start: "2018-02-23T12:30:00",
                        end: "2018-02-24T12:30:00",
                        color: "#e16123",
                        id: "demo4"
                    }, {
                        title: "午饭",
                        start: "2018-02-24T13:30:00",
                        id: "demo5"
                    }, {
                        title: "旅游",
                        start: "2018-02-25",
                        color: "#ff4040",
                        id: "demo6"
                    }
                ],
                eventClick: function (calEvent, jsEvent, view) {
                    //e.fullCalendar('removeEventSource', calEvent)
                    console.log(calEvent);
                    console.log(calEvent.id);
                    console.log(jsEvent);
                    console.log(view);
           
                    console.log("qqqq");
                },
                // 点击某一天响应事件
                dayClick: function (date, jsEvent, view, resourceObj) {
                    console.log(date.format());
                    alert("新增:" + date.format());
                    var newEvents = [];
                    var event = new Object();
                    event = {
                        title: "ewewqwqwqeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeessssss",
                        start: date.format()+" 12:30:00",
                        id: "100",
                        color: "#ff4040",
                    };
                    newEvents.push(event);
                    $('#calendarApp').fullCalendar('addEventSource', newEvents);
                }
            });
        }
        a(document).on("click", ".btn_del", function (date, jsEvent, view) {
            console.log(date);
            console.log(jsEvent);
            console.log(view);
            console.log("删除数据");
            //a(document).fullCalendar('refetchEvents');
            //console.log(e.fullCalendar('getEventSources')[0].rawEventDefs[0]);
            ////e.fullCalendar('removeEvents', "demo6");
            
            //e.fullCalendar('removeEventSource', e.fullCalendar('getEventSources')[0].rawEventDefs[0]);
            
        });
    });
}(jQuery));
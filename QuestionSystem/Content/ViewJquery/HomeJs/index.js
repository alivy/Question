var data = [{
    text: '一级菜单',
    iconCls: 'fa fa-wpforms',
    state: 'open',
    children: [{
        //text: '/ Question / Question<ul><li><div class="menunav navselected"><a ref="1" href="javascript:void(0)"  rel="/Question/Index"><span class="icon {2}">&nbsp;</span><span class="nav" >题目列表</span></a></div></li></ul>'
        text: '<a class="nav" rel="/Question/Index" style="text-decoration:none;" >订单列表 </a>'
    }, {
        text: '<a class="nav" rel="/Question/Prize" style="text-decoration:none;"   onclick="addTab("奖品列表", "/Question/Prize", icon)" >奖品列表 </a>'
    }, {
        text: '<a class="nav" rel="/Question/CetDate" style="text-decoration:none;" >领取时间列表 </a>'

    }, {
        text: '<a class="nav" rel="/Question/Question" style="text-decoration:none;" >题目列表 </a>'
    }]
}
    //    , {
    //    text: 'Mail',
    //    iconCls: 'fa fa-at',
    //    selected: true,
    //    children: [{
    //        text: '二级子菜单'
    //    }, {
    //        text: '二级子菜单'
    //    }, {
    //        text: '二级父菜单',
    //        children: [{
    //            text: '三级子菜单'
    //        }, {
    //            text: '三级子菜单'
    //        }]
    //    }]
    //}, {
    //    text: 'Layout',
    //    iconCls: 'fa fa-table',
    //    children: [{
    //        text: '二级子菜单'
    //    }, {
    //        text: '二级子菜单'
    //    }, {
    //        text: '二级子菜单'
    //    }]
    //    }
];

//菜单生成
//function menuAccordion(menus) {
//    var $obj = $('#sm');
//    $obj.accordion({ animate: false, fit: true, border: false });
//    $.each(menus, function () {
//        var html = '<ul>';
//        var temple = '<li><div><a ref="{0}" href="javascript:void(0)" rel="{1}"><span class="icon {2}">&nbsp;</span><span class="nav">{3}</span></a></div></li>';
//        $.each(this.children || [], function () {
//            html += utils.formatString(temple, this.MenuCode, this.URL, this.IconClass, this.MenuName);
//        });
//        html += '</ul>';

//        $obj.accordion('add', {
//            title: this.MenuName,
//            content: html,
//            iconCls: 'icon ' + this.IconClass,
//            border: false
//        });
//    });

//    var panels = $obj.accordion('panels');
//    $obj.accordion('select', panels[0].panel('options').title);

//    $obj.find('li').click(function () {
//        $obj.find('li div').removeClass("navselected");
//        $(this).children('div').addClass("navselected");

//        var link = $(this).find('a');
//        var title = link.children('.nav').text();
//        var url = link.attr("rel");
//        var code = link.attr("ref");
//        var icon = link.children('.icon').attr('class');
//        addTab(title, url, icon);
//    }).hover(function () {
//        $(this).children('div').addClass("hover");
//    }, function () {
//        $(this).children('div').removeClass("hover");
//    });
//};



//折叠菜单
function toggle() {
    var opts = $('#sm').sidemenu('options');
    $('#sm').sidemenu(opts.collapsed ? 'expand' : 'collapse');
    opts = $('#sm').sidemenu('options');
    $('#sm').sidemenu('resize', {
        width: opts.collapsed ? 60 : 200
    })
}

$(function () {
    // 初始化内容
    //addTab("个人设置", "/sys/config", "icon icon-wrench_orange");

    $('#sm').find('li').click(function () {
        var $obj = $('#sm');
        $obj.find('li div').removeClass("selected");
        $(this).children('div').addClass("selected");
        var link = $(this).find('a');
        var title = link.text();
        var url = link.attr("rel");
        var code = link.attr("ref");
        var icon = link.children('.icon').attr('class');
        addTab(title, url, icon);
    }).hover(function () {
        $(this).children('div').addClass("hover");
    }, function () {
        $(this).children('div').removeClass("hover");
    });
});

//添加tab页
function addTab(subtitle, url, icon) {
    if (!url || url == '#') return false;
    var $tab = $('#tabs');
    var tabCount = $tab.tabs('tabs').length;
    var hasTab = $tab.tabs('exists', subtitle);
    if ((tabCount <= 8) || hasTab)
        openTabHandler($tab, hasTab, subtitle, url, icon);
    else
        $.messager.confirm("系统提示", '<b>您当前打开了太多的页面，如果继续打开，会造成程序运行缓慢，无法流畅操作！</b>', function (b) {
            if (b)
                openTabHandler($tab, hasTab, subtitle, url, icon);
        });
};


function openTabHandler($tab, hasTab, subtitle, url, icon) {
    if (!hasTab) {
        $tab.tabs('add', { title: subtitle, content: createFrame(url), closable: true, icon: icon });
    } else {
        $tab.tabs('select', subtitle);
        tabRefresh(url);   //选择TAB时刷新页面
    }
    setLocationHash();
};

//添加内容
function createFrame(url) {
    return '<iframe scrolling="auto" frameborder="0"  style="width:100%;height:100%;" src="' + url + '" ></iframe>';
}


function setLocationHash() {
    try {
        var $obj = $('#tabs');
        var src = '', tabs = $obj.data().tabs.tabs;
        var tab = $obj.tabs('getSelected');

        var fnSrc = function (tab) {
            var iframe = tab.find('iframe');
            return iframe.length ? iframe[0].src.replace(location.host, '').replace('http://', '').replace('.aspx', '') : '';
        };

        if (tab) {
            src = fnSrc(tab);
            if (src) window.location.hash = '!' + src;   //如果src没有，就不设置，case在f5刷新时出现
            if (src == homeUrl) window.location.hash = '';
        }
        else {
            src = fnSrc(tabs[tabs.length - 1]); //关闭tabs时，当前tab为空
            window.location.hash = '!' + src;
        }
    }
    catch (e) { }
}

///刷新tab页
function tabRefresh(url) {
    var $tab = $("#tabs");
    var currentTab = $tab.tabs('getSelected');
    var src = $(currentTab.panel('options').content).attr('src');
    if (typeof src === 'string') src = url;
    $tab.tabs('update', { tab: currentTab, options: { content: createFrame(src) } })
};
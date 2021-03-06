import Vue from 'vue'
import App from './App.vue'
//vuex 属性状态统一管理
import store from './vuex/store';
//router 路由
import router from './js/router/index';
//element ui
import ElementUI from 'element-ui';
import './css/element-variables.scss';
//中型
// Vue.use(ElementUI, { size: 'medium', zIndex: 1000 });
//小型
Vue.use(ElementUI, { size: 'small', zIndex: 1000 });
//font
import './assets/font/iconfont.css'
//http request
import {get, post, upload, download } from './js/request'
//tools 工具
import tools from './js/tools'
//global 配置
global.$store = store;
global.$router = router;
global.tools = tools;
global.get = get;
global.post = post;
global.upload = upload;
global.download = download;
//记录打开的子页面 权限对象
global.$power = {};
//
Vue.config.productionTip = false;
//
new Vue({
    store,
    router, //加入我自己的路由
    render: h => h(App)
}).$mount('#app')
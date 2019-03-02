import Vue from 'vue';
import Vuetify from 'vuetify';
import ru from 'vuetify/es5/locale/ru';
import App from './App.vue';
import store from './store';
import router from './router';
import 'vuetify/dist/vuetify.min.css'; 
import Vuelidate from 'vuelidate';
import '@fortawesome/fontawesome-free/css/all.min.css';

Vue.use(Vuelidate);

Vue.config.productionTip = false;
//Vue.use(Element, {locale});
Vue.use(Vuetify, {
    lang: {
        locales: { ru },
        current: 'ru'
    }
});

new Vue({
    store,
    router,
    render: h => h(App)
}).$mount('#app');

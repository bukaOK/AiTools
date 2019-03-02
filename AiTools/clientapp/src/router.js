import Vue from 'vue'
import Register from './views/Register.vue'
import Login from './views/Login.vue'
import Home from './views/Home.vue'
import VueRouter from 'vue-router'
import PayOperations from './views/PayOperations.vue'
import PayIn from './views/PayIn.vue'
import SeasonForecast from './views/SeasonForecast.vue'

Vue.use(VueRouter)

export default new VueRouter({
    mode: 'history',
    base: process.env.BASE_URL,
    routes: [
        {
            path: '/register',
            name: 'register',
            component: Register
        },
        {
            path: '/',
            name: 'home',
            component: Home
        },
        {
            path: '/login',
            name: 'login',
            component: Login
        },
        {
            path: '/forecasts/season',
            name: 'seasonForecast',
            component: SeasonForecast
        },
        {
            path: '/operations',
            name: 'payOperations',
            component: PayOperations
        },
        {
            path: '/payin',
            name: 'payIn',
            component: PayIn
        }
    ]
})
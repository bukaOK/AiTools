import Vue from 'vue'
import Register from './views/Register.vue'
import Login from './views/Login.vue'
import Home from './views/Home.vue'
import VueRouter from 'vue-router'
import DataAnalyze from './views/DataAnalyze'
import AnalyzeResults from './views/AnalyzeResults'
import AddForecast from './views/AddForecast'
import ForecastList from './views/ForecastList'
import Employees from './views/Employees'

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
            path: '/analyze/data/',
            name: 'dataAnalyze',
            component: DataAnalyze
        },
        {
            path: '/analyze/data/:id',
            name: 'dataAnalyzeEdit',
            props: true,
            component: DataAnalyze
        },
        {
            path: '/analyze/results',
            name: 'analyzeResults',
            component: AnalyzeResults
        },
        {
            path: '/forecasts/new',
            name: 'addForecast',
            component: AddForecast
        },
        {
            path: '/forecasts/edit/:id',
            name: 'editForecast',
            component: AddForecast
        },
        {
            path: '/forecasts/results',
            name: 'forecastList',
            component: ForecastList
        },
        {
            path: '/employees',
            name: 'employees',
            component: Employees
        }
    ]
})
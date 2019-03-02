import Vue from 'vue';
import Vuex from 'vuex';
import axios from 'axios';

Vue.use(Vuex);

const apiConfig = process.env.NODE_ENV == 'development' ? {
    baseURL: 'http://localhost:5000/',
    withCredentials: true
} : {
    baseURL: '/'
};
export default new Vuex.Store({
    state: {
        authenticated: false,
        userName: null,
        userCash: '0.00 р',
        api: axios.create(apiConfig),
        containerLoading: true
    },
    mutations: {
        setUserState(state, payload){
            state.authenticated = payload.authenticated;
            state.userName = payload.userName;
            state.userCash = payload.userCash;
        },
        beginContainerLoad(state){
            state.containerLoading = true;
        },
        finishContainerLoad(state){
            state.containerLoading = false;
        }
    },
    actions: {
        //Заполняем данные о пользователе
        fillUserState({commit, state}, payload){
            commit('beginContainerLoad');
            state.api.get('/clientstate')
                .then(resp => {
                    if(payload && payload.onSuccess){
                        payload.onSuccess(resp);
                    }
                    commit('setUserState', {
                        authenticated: resp.data.UserAuthenticated,
                        userName: resp.data.UserName,
                        userCash: resp.data.UserCash
                    });
                    commit('finishContainerLoad');
                }).catch(() => {
                    commit('finishContainerLoad');
                });
        }
    }
})
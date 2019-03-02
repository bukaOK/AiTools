<template>
    <v-dialog v-model="dialogVisible" width="300" persistent>
        <v-toolbar color="primary" dark>
            <v-toolbar-title>Пополнение счета</v-toolbar-title>
            <v-spacer />
            <v-btn icon @click="hideDialog">
                <v-icon>close</v-icon>
            </v-btn>
        </v-toolbar>
        <div v-if="loading">
            <v-progress-circular indeterminate color="primary" />
        </div>
        <v-card v-else>
            <v-window v-model="step">
                <v-window-item value="list">
                    <v-list>
                        <v-list-tile v-for="type in payTypes" :key="type.type" @click="setPayType(type)">
                            <v-list-tile-avatar>
                                <img v-if="type.image" :src="type.image" :alt="type.title" />
                                <v-icon v-else-if="type.icon">{{type.icon}}</v-icon>
                            </v-list-tile-avatar>
                            <v-list-tile-content>
                                <v-list-tile-title>{{type.title}}</v-list-tile-title>
                            </v-list-tile-content>
                        </v-list-tile>
                    </v-list>
                </v-window-item>
                <!--Yandex window-->
                <v-window-item value="sumForm">
                    <v-card-text>
                        <form>
                            <v-text-field label="Сумма" v-model="sum" />
                        </form>
                    </v-card-text>
                    <v-card-actions>
                        <v-btn icon @click="backToList">
                            <v-icon>arrow_back</v-icon>
                        </v-btn>
                        <v-spacer />
                        <v-btn color="primary">Пополнить</v-btn>
                    </v-card-actions>
                </v-window-item>
                <v-window-item value="yandexAuth">
                    <v-card-text>
                        <div class="headline text-xs-center">Необходимо авторизовать приложение в Яндекс.Деньгах</div>
                        <form action="https://money.yandex.ru/oauth/authorize">
                            <input type="hidden" :value="yandexSettings.clientId" name="client_id" />
                            <input type="hidden" :value="yandexSettings.redirectUri" name="redirect_uri" />
                            <input type="hidden" :value="yandexSettings.scope" name="scope" />
                            <v-btn block type="submit">Авторизовать</v-btn>
                        </form>
                    </v-card-text>
                </v-window-item>
            </v-window>
        </v-card>
    </v-dialog>
</template>
<script>
import {required, integer} from 'vuelidate/lib/validators';
import yaLogo from '../assets/images/yaMoneyIcon.png';
import qiwiLogo from '../assets/images/qiwiIcon.png';
import wmLogo from '../assets/images/wmlogo_flat_128.png';
import { mapState } from 'vuex';

export default {
    name: 'pay-in-dialog',
    loading: true,
    props: {
        dialogVisible: Boolean
    },
    created(){
        this.api.get('/yandex/authdata').then(resp => {
            this.yandexSettings.token = resp.data.Token;
            this.yandexSettings.clientId = resp.data.ClientId;
            this.yandexSettings.redirectUri = resp.data.RedirectUri;
            this.yandexSettings.scope = resp.data.Scope;
            
            this.loading = false;
        }).catch(() => {
            this.loading = false;
        });
    },
    validations: {
        sum: {required, integer}
    },
    data(){
        return {
            step: 'list',
            currentPayInType: null,
            yandexSettings: {
                token: String,
                clientId: String,
                redirectUri: String,
                scope: String
            },
            //Поля формы
            sum: null,
            payTypes: [
                {
                    title: 'Яндекс.Деньги',
                    type: 'yandex',
                    image: yaLogo,
                    step: 'sumForm'
                },
                {
                    title: 'QIWI',
                    type: 'qiwi',
                    image: qiwiLogo,
                    step: 'sumForm'
                },
                {
                    title: 'WebMoney',
                    type: 'wm',
                    image: wmLogo,
                    step: 'sumForm'
                },
                {
                    title: 'Банковская карта',
                    type: 'bank',
                    icon: 'fas fa-credit-card',
                    step: 'sumForm'
                }
            ]
        }
    },
    methods: {
        hideDialog(){
            this.$emit('hide-dialog');
            this.step = 0;
        },
        /**
         * Обработка клика на вид платежа
         * @typedef {object} PayType 
         * @property {string} title
         * @property {string} type
         * @property {object} image
         * @property {string} step
         */
        /**
         * @param {PayType} type
         */
        setPayType(type){
            if(type.type === 'yandex' && !this.yandexSettings.token){
                this.step = 'yandexAuth';
            } else{
                this.step = type.step;
            }
            this.currentPayInType = type.type;
        },
        /**
         * Возврат к списку видов платежей
         */
        backToList(){
            this.step = 'list';
            this.currentPayInType = null;
        }
    },
    computed: mapState([
        'api'
    ])
}
</script>


<template>
    <div>
        <v-app v-if="containerLoading">
            <v-container fill-height>
                <v-layout row justify-center align-center>
                    <v-progress-circular indeterminate :size="70" :width="7" color="indigo" />
                </v-layout>
            </v-container>
        </v-app>
        <v-app v-else>
            <v-navigation-drawer fixed clipped app v-if="authenticated">
                <v-list>
                    <template v-for="(item, ind) in drawerItems">
                        <v-list-group v-if="item.isGroup" :key="ind" append-icon="" v-model="item.isOpen"
                            :prepend-icon="item.isOpen ? item.openIcon : item.closeIcon">
                            <v-list-tile slot="activator">
                                <v-list-tile-content>
                                    <v-list-tile-title>{{item.title}}</v-list-tile-title>
                                </v-list-tile-content>
                            </v-list-tile>
                            <v-list-tile v-for="(nestItem, nestInd) in item.items" :key="nestInd"
                                @click="nestItem.onClick">
                                <v-list-tile-action>
                                    <v-icon>{{ nestItem.icon }}</v-icon>
                                </v-list-tile-action>
                                <v-list-tile-content>
                                    <v-list-tile-title>{{ nestItem.title }}</v-list-tile-title>
                                </v-list-tile-content>
                            </v-list-tile>
                        </v-list-group>
                        <v-list-tile v-else :key="ind" @click="item.onClick">
                            <v-list-tile-action>
                                <v-icon>{{ item.icon }}</v-icon>
                            </v-list-tile-action>
                            <v-list-tile-content>
                                <v-list-tile-title>{{ item.title }}</v-list-tile-title>
                            </v-list-tile-content>
                        </v-list-tile>
                    </template>
                </v-list>
            </v-navigation-drawer>
            <v-toolbar app color="indigo" dark clipped-left>
                <v-toolbar-title>Ai Tools</v-toolbar-title>
                <v-spacer></v-spacer>
                <!-- <pay-info /> -->
                <div v-if="authenticated">
                    <v-menu>
                        <v-toolbar-title slot="activator" class="white--text">
                            {{userName}}
                        </v-toolbar-title>
                        <v-list>
                            <v-list-tile @click="logOff">
                                <v-list-tile-title>Выход</v-list-tile-title>
                            </v-list-tile>
                        </v-list>
                    </v-menu>
                </div>
                <div v-else>
                    <v-btn outline to="/register">Регистрация</v-btn>
                    <v-btn color="blue" to="/login">Войти</v-btn>
                </div>
            </v-toolbar>
            <v-content>
                <v-container>
                    <router-view></router-view>
                </v-container>
            </v-content>
        </v-app>
    </div>
</template>
<script>
    import {mapState, mapActions} from 'vuex';
    import PayInfo from '../components/PayInfo.vue';

    export default {
        name: 'layout',
        components: {
            PayInfo
        },
        data(){
            return{
                logOffError: false,
                drawer: true,
                drawerItems: [
                    {
                        closeIcon: 'fas fa-angle-down',
                        openIcon: 'fas fa-angle-up',
                        title: 'Прогнозы',
                        isGroup: true,
                        isOpen: false,
                        items: [
                            {
                                title: 'Сезонный прогноз спроса',
                                icon: 'fas fa-cloud-sun-rain',
                                onClick: () => {
                                    this.$router.push('/forecasts/season')
                                }
                            }
                        ]
                    },
                    { 
                        title: 'История операций', 
                        icon: 'fas fa-history',
                        onClick: () => {
                            this.$router.push('/operations')
                        }
                    },
                    {
                        title: 'Пополнить счет',
                        icon: 'fas fa-plus-square',
                        onClick: () => {
                            this.$router.push('/payin')
                        }
                    },
                    {
                        title: 'Вывести средства',
                        icon: 'fas fa-minus-square'
                    },

                ]
            };
        },
        created(){
            this.fillUserState();
        },
        methods: {
            logOff(){
                const router = this.$router;
                this.api.get('/account/logoff').then(() => {
                    this.fillUserState({
                        onSuccess: () => {
                            router.push('/home');
                        }
                    });
                }).catch(() => {
                    this.logOffError = true;
                    setTimeout(() => (this.logOffError = false), 2000);
                });
            },
            ...mapActions([
                'fillUserState'
            ])
        },
        computed: {
            ...mapState([
                'authenticated',
                'containerLoading',
                'userName',
                'api'
            ])
        }
    }
</script>
<style>
.loading-container{
    background: #fff;
}
</style>

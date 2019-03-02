<template>
    <v-layout align-center justify-center>
        <v-flex xs12 sm8 md4>
            <v-card>
                <v-toolbar dark color="primary">
                    <v-toolbar-title>Вход</v-toolbar-title>
                </v-toolbar>
                <v-card-text>
                    <form @keypress="formKeyPress">
                        <v-text-field prepend-icon="person" label="E-mail" 
                            :error-messages="emailErrors" v-model="email" />
                        <v-text-field prepend-icon="lock" label="Пароль" type="password"
                            :error-messages="passwordErrors" v-model="password" />
                    </form>
                </v-card-text>
                <v-card-actions>
                    <v-btn flat>Я забыл пароль!</v-btn>
                    <v-spacer />
                    <v-btn color="primary" @click="login" :loading="loading">Войти</v-btn>
                </v-card-actions>
                <div v-if="errors.length">
                    <v-alert v-for="(error, ind) in errors" :value="true" type="error" :key="ind">
                        {{error}}
                    </v-alert>
                </div>
            </v-card>
        </v-flex>
    </v-layout>
</template>

<script>
    import formHelper from'../helpers/formHelper.js';
    import {mapState} from 'vuex';
    import {required, email} from 'vuelidate/lib/validators';

    export default {
        validations: {
            email: { required, email },
            password: { required }
        },
        name: 'login',
        data(){
            return {
                errors: [],
                loading: false,
                email: '',
                password: ''
            }
        },
        computed: {
            emailErrors(){
                const errors = [];
                if (this.$v.email.$dirty) {
                    if(!this.$v.email.required)
                        errors.push('Заполните e-mail');
                    if(!this.$v.email.email)
                        errors.push('Неверный e-mail');
                }
                return errors;
            },
            passwordErrors(){
                const errors = [];
                if(this.$v.password.$dirty){
                    if(!this.$v.password.required)
                        errors.push('Заполните пароль');
                }
                return errors;
            },
            ...mapState([
                'api'
            ])
        },
        methods: {
            formKeyPress(ev){
                if(ev.key === 'Enter'){
                    this.login();
                }
            },
            login() {
                this.$v.$touch();
                if(!this.$v.$invalid) {
                    const fData = new FormData();
                    fData.append('Email', this.email);
                    fData.append('Password', this.password);
                    
                    this.loading = true;

                    //begin request
                    this.api.post('/account/login', fData).then(() => {
                        this.$store.dispatch('fillUserState', {
                            onSuccess: () => {
                                this.$router.push('/home');
                            }
                        });
                        this.loading = false;
                    }).catch(err => {
                        if(err.response.status === 400){
                            this.errors = formHelper.parseErrors(err.response.data);
                        } else{
                            this.errors = ['Внутренняя ошибка'];
                        }
                        this.loading = false;
                    });
                }
            }
        }
    }
</script>

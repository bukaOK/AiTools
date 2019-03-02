<template>
    <v-layout align-center justify-center>
        <v-flex xs12 sm8 md4>
            <v-card>
                <v-toolbar dark color="primary">
                    <v-toolbar-title>Регистрация</v-toolbar-title>
                </v-toolbar>
                <v-card-text>
                    <form @keypress="formKeyPress">
                        <v-text-field label="Имя" v-model="name" :error-messages="nameErrors" />
                        <v-text-field label="Фамилия" v-model="sirname" :error-messages="sirnameErrors" />
                        <v-dialog ref="dialog" v-model="bdModal" :return-value.sync="birthday" persistent 
                            lazy full-width width="290px">
                            <v-text-field slot="activator" v-model="birthdayFormatted" label="Дата рождения" 
                                 readonly :error-messages="birthdayErrors" />
                            <v-date-picker v-model="birthday" scrollable locale="ru">
                                <v-spacer></v-spacer>
                                <v-btn flat color="primary" @click="bdModal = false">Отмена</v-btn>
                                <v-btn flat color="primary" @click="$refs.dialog.save(birthday)">Ок</v-btn>
                            </v-date-picker>
                        </v-dialog>
                        <v-text-field label="E-mail" :error-messages="emailErrors" v-model="email" />
                        <v-text-field label="Пароль" type="password" :error-messages="passwordErrors" v-model="password" />
                        <v-text-field label="Подтвердите пароль" type="password" v-model="passwordConfirm"
                             :error-messages="passwordConfirmErrors" />
                    </form>
                </v-card-text>
                <v-card-actions>
                    <v-spacer />
                    <v-btn color="primary" :loading="loading" @click="register">Зарегистрироваться</v-btn>
                </v-card-actions>
                <div v-if="errors.length">
                    <v-alert v-for="error in errors" :value="true" type="error" :key="error.id">
                        {{error.message}}
                    </v-alert>
                </div>
            </v-card>
        </v-flex>
    </v-layout>
</template>

<script>
import formHelper from '../helpers/formHelper.js';
import {mapState} from 'vuex';
import {required, email, sameAs, minLength} from 'vuelidate/lib/validators';

export default {
    name: 'register',
    validations: {
        name: {
            required,
            requireWords: value => {
                if (typeof value === 'undefined' || value === null || value === '') {
                    return true;
                }
                return /[a-zA-Z]*/.test(value);
            }
        },
        sirname: {
            required,
            requireWords: value => {
                if (typeof value === 'undefined' || value === null || value === '') {
                    return true;
                }
                return /[a-zA-Z]*/.test(value);
            }
        },
        email: { required, email },
        password: { 
            required,
            requireDigits: value => {
                if (typeof value === 'undefined' || value === null || value === '') {
                    return true;
                }
                return /[0-9]+/.test(value);
            },
            requireWords: value => {
                if (typeof value === 'undefined' || value === null || value === '') {
                    return true;
                }
                return /[a-zA-Z]*/.test(value);
            },
            minLength: minLength(6)
        },
        passwordConfirm: {
            required,
            sameAsPassword: sameAs('password')
        },
        birthday: {
            required,
            minAge: bdStr => {
                const today = new Date();
                const birthDate = new Date(bdStr);
                let age = today.getFullYear() - birthDate.getFullYear();
                const m = today.getMonth() - birthDate.getMonth();

                if (m < 0 || (m === 0 && today.getDate() < birthDate.getDate())) {
                    age--;
                }
                if(age < 14)
                    return false;
                return true;
            }
        }
    },
    data() {
        return {
            errors: [],
            loading: false,

            email: '',
            password: '',
            //В формате ISO
            birthday: '',
            birthdayFormatted: '',
            passwordConfirm: '',
            name: '',
            sirname: '',
            bdModal: false,
        }
    },
    computed: {
        nameErrors(){
            const errors = [];

            if (this.$v.name.$dirty) {
                if(!this.$v.name.required)
                    errors.push('Заполните имя');
                if(!this.$v.name.requireWords)
                    errors.push('Только русские или латинские буквы');
            }
            return errors;
        },
        sirnameErrors(){
            const errors = [];

            if (this.$v.sirname.$dirty) {
                if(!this.$v.sirname.required)
                    errors.push('Заполните фамилию');
                if(!this.$v.sirname.requireWords)
                    errors.push('Только русские или латинские буквы');
            }
            return errors;
        },
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
                if(!this.$v.password.requireWords)
                    errors.push('Пароль должен содержать буквы');
                if(!this.$v.password.requireDigits)
                    errors.push('Пароль должен содержать цифры');
                if(!this.$v.password.minLength)
                    errors.push(`Минимальная длина пароля ${this.$v.password.$params.minLength.min} символов`);
            }
            return errors;
        },
        passwordConfirmErrors(){
            const errors = [];
            if(this.$v.passwordConfirm.$dirty){
                if(!this.$v.passwordConfirm.required)
                    errors.push('Заполните подтверждение пароля');
                if(!this.$v.passwordConfirm.sameAsPassword)
                    errors.push('Пароли не совпадают');
            }
            return errors;
        },
        birthdayErrors(){
            const errors = [];
            const bd = this.$v.birthday;

            if(bd.$dirty){
                if(!bd.required)
                    errors.push('Заполните дату рождения');
                if(!bd.minAge)
                    errors.push('Вам нет 14 лет');
            }
            return errors;
        },
        ...mapState([
            'api'
        ])
    },
    watch: {
        birthday(val){
            this.birthdayFormatted = this.formatDate(val);
        }
    },
    methods: {
        formKeyPress(ev){
            if(ev.key === 'Enter'){
                this.login();
            }
        },
        formatDate(date){
            if(!date)
                return null;
            const [year, month, day] = date.split('-');
            return `${day}.${month}.${year}`;
        },
        register(formName){
            this.$v.$touch();
            if(!this.$v.$invalid){
                const data = new FormData();

                data.append('Email', this.email);
                data.append('Password', this.password);
                data.append('PasswordConfirm', this.passwordConfirm);
                data.append('BirthDate', this.birthdayFormatted);
                data.append('Name', this.name);
                data.append('SirName', this.sirname);
                
                this.loading = true;
                this.api.post('/account/register', data).then(() => {
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
                        this.errors.push('Внутренняя ошибка');
                    }
                    this.loading = false;
                });
            }
        }
    }
}
</script>
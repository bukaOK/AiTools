<template>
    <v-layout align-center justify-center>
        <v-flex xs12 sm8 md4>
            <v-card>
                <v-toolbar dark color="primary">
                    <v-toolbar-title>Регистрация</v-toolbar-title>
                </v-toolbar>
                <v-form @submit.prevent="register" ref="form">
                    <v-card-text>
                        <v-text-field label="Имя" v-model="name" :rules="rules.name" />
                        <v-text-field label="Фамилия" v-model="sirname" :rules="rules.sirname" />
                        <v-text-field label="E-mail" :rules="rules.email" v-model="email" />
                        <v-text-field label="Пароль" type="password" :rules="rules.password" v-model="password" />
                        <v-text-field label="Подтвердите пароль" type="password" v-model="passwordConfirm"
                            :rules="rules.passwordConfirm" />
                    </v-card-text>
                    <v-card-actions>
                        <v-spacer />
                        <v-btn color="primary" :loading="loading" type="submit">Зарегистрироваться</v-btn>
                    </v-card-actions>
                </v-form>
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
import formHelper from '../helpers/formHelper.js'
import {mapState} from 'vuex'

const validations = formHelper.validations
export default {
    name: 'register',
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
            rules: {
                email: [
                    v => validations.required(v, 'E-mail'),
                    v => validations.email(v)
                ],
                password: [
                    v => validations.required(v, 'Пароль'),
                    v => /[0-9]+/.test(v) || 'Пароль должен содержать цифры',
                    v => v.length < 6 || 'Длина пароля должна быть больше 6'
                ],
                passwordConfirm: [
                    v => validations.required(v, 'Потвердите пароль'),
                    v => v === this.password || 'Пароли не совпадают'
                ],
                inviteKey: [
                    v => validations.required(v, 'Ключ приглашения')
                ],
                name: [
                    v => validations.required(v, 'Имя'),
                    v => /^[а-яА-Я]*$/.test(v) || 'Имя должно быть кириллицей'
                ],
                name: [
                    v => validations.required(v, 'Фамилия'),
                    v => /^[а-яА-Я]*$/.test(v) || 'Фамилия должна быть кириллицей'
                ],
            }
        }
    },
    computed: {
        ...mapState([
            'api'
        ])
    },
    methods: {
        register(){
            if(!this.$refs.form.validate()){
                const data = new FormData()

                data.append('Email', this.email)
                data.append('Password', this.password)
                data.append('PasswordConfirm', this.passwordConfirm)
                data.append('BirthDate', this.birthdayFormatted)
                data.append('Name', this.name)
                data.append('SirName', this.sirname)
                
                this.loading = true
                this.api.post('/account/register', data).then(() => {
                    this.$store.dispatch('fillUserState', {
                        onSuccess: () => {
                            this.$router.push('/home')
                        }
                    })
                    this.loading = false
                }).catch(err => {
                    if(err.response.status === 400){
                        this.errors = formHelper.parseErrors(err.response.data)
                    } else{
                        this.errors.push('Внутренняя ошибка')
                    }
                    this.loading = false
                })
            }
        }
    }
}
</script>
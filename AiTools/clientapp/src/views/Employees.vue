<template>
    <v-container grid-list-lg class="ma-0">
        <v-layout>
            <v-flex xs12 md8>
                <v-card>
                    <v-card-title class="headline">Сотрудники</v-card-title>
                    <v-card-text>
                        <v-data-table :headers="headers" :items="items" :loading="tableLoading">
                            <template v-slot:items="props">
                                <td>{{ props.item.name }}</td>
                                <td class="text-xs-right">{{ props.item.sirName }}</td>
                                <td class="text-xs-right">{{ props.item.email }}</td>
                            </template>
                        </v-data-table>
                    </v-card-text>
                </v-card>
            </v-flex>
            <v-flex xs12 md4>
                <v-card>
                    <v-card-text>
                        <v-text-field label="Ключ приглашения" v-model="inviteKey" />
                        <v-btn block color="primary" :loading="inviteLoading" @click="createKey">Сгенерировать ключ приглашения</v-btn>
                    </v-card-text>
                </v-card>
            </v-flex>
        </v-layout>
    </v-container>
</template>
<script>
import { mapState } from 'vuex'

export default {
    mounted(){
        this.api.get('/employee/getall').then(resp => {
            for(let i = 0; i < resp.data.length; i++){
                this.items.push(resp.data[i])
            }
        }).finally(() => {
            this.tableLoading = false
        })
        this.api.get('/employee/getinvitekey').then(resp => {
            this.inviteKey = resp.data
        })
    },
    data(){
        return {
            inviteKey: '',
            inviteLoading: false,
            tableLoading: true,
            items: [],
            headers: [
                { text: 'Имя', value: 'name' },
                { text: 'Фамилия', value: 'sirName' },
                { text: 'E-mail', value: 'email' }
            ]
        }
    },
    computed: {
        ...mapState([
            'api'
        ])
    },
    methods: {
        createKey(){
            this.inviteLoading = true
            this.api.get('/employee/createinvitekey')
                .then(resp => {
                    this.inviteKey = resp.data
                }).finally(() => {
                    this.inviteLoading = false
                })
        }
    }
}
</script>

# Generated by Django 4.0.1 on 2022-01-19 13:10

from django.db import migrations, models


class Migration(migrations.Migration):

    initial = True

    dependencies = [
    ]

    operations = [
        migrations.CreateModel(
            name='Activity',
            fields=[
                ('id', models.BigAutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('name', models.CharField(max_length=64)),
                ('content', models.TextField()),
                ('author', models.CharField(max_length=64)),
                ('cover', models.CharField(max_length=255)),
                ('tags', models.CharField(max_length=64)),
                ('status', models.CharField(default='open', max_length=64)),
                ('time_slot', models.CharField(max_length=64)),
                ('place', models.CharField(max_length=64)),
                ('created_at', models.DateTimeField(auto_now_add=True)),
                ('updated_at', models.DateTimeField(auto_now=True)),
            ],
            options={
                'db_table': 'activities',
            },
        ),
    ]
